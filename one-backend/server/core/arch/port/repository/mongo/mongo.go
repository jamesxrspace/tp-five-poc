package mongo

import (
	"context"
	"fmt"
	"reflect"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
)

func FindByID[T any](ctx context.Context, collection *mongo.Collection, id string) (T, error) {
	var result T

	// 當 T 不是 pointer type 的時後 `var result T` 會建立一個 instance
	// 在 FindByID 的設計，在 no document 的時後，會希望回傳 nil, nil
	// 這個時後如果不是 pointer type 的時後，會無法達成
	if reflect.TypeOf(result).Kind() != reflect.Ptr {
		return result, core_error.StackError("type must be a pointer")
	}

	pkDetail, err := getPKDetails(result)
	if err != nil {
		return result, err
	}

	c := collection.FindOne(ctx, bson.M{pkDetail.bsonName: id})
	if c.Err() != nil {
		if c.Err() == mongo.ErrNoDocuments {
			return result, nil
		}
		return result, core_error.StackError(c.Err())
	}

	// 原來的寫法是 err = c.Decord(result), 但這個時後 result 是 nil 會出錯, 所以要先 new 一個出來
	r := new(T)
	err = c.Decode(r)
	if err != nil {
		return result, core_error.StackError(err)
	}

	return *r, nil
}

func FindList[T any](ctx context.Context, collection *mongo.Collection, filter bson.M, query *pagination.PaginationQuery, sort bson.M) (result []T, Total int, errRet error) {
	option := options.Find()
	if query != nil {
		o := int64(query.Offset)
		option.Skip = &o
		s := int64(query.Size)
		option.Limit = &s
	}

	if sort != nil {
		option.SetSort(sort)
	}

	curr, err := collection.Find(ctx, filter, option)
	if err != nil {
		return nil, 0, core_error.StackError(err)
	}

	result = make([]T, 0)
	err = curr.All(ctx, &result)
	if err != nil {
		return nil, 0, core_error.StackError(err)
	}

	total, err := collection.CountDocuments(ctx, filter)
	if err != nil {
		return nil, 0, core_error.StackError(err)
	}

	return result, int(total), nil
}

func FindByFilter[T any](ctx context.Context, collection *mongo.Collection, filter bson.M) (T, error) {
	var result T

	if reflect.TypeOf(result).Kind() != reflect.Ptr {
		return result, core_error.StackError("type must be a pointer")
	}

	c := collection.FindOne(ctx, filter)
	if c.Err() != nil {
		if c.Err() == mongo.ErrNoDocuments {
			return result, nil
		}
		return result, core_error.StackError(c.Err())
	}

	r := new(T)
	err := c.Decode(r)
	if err != nil {
		return result, core_error.StackError(err)
	}

	return *r, nil
}

func InsertOne(ctx context.Context, collection *mongo.Collection, item interface{}) (*mongo.InsertOneResult, error) {
	return collection.InsertOne(ctx, item)
}

func InsertMany[T any](ctx context.Context, collection *mongo.Collection, items []T) (*mongo.InsertManyResult, error) {
	// https://eric-zx.medium.com/golang-convert-type-to-interface-482de13ed57c
	// We can't convert []*T to []interface{} directly, so we need to convert it one by one

	convert := make([]interface{}, len(items))
	for i, item := range items {
		convert[i] = item
	}

	return collection.InsertMany(ctx, convert)
}

func Save[T any](ctx context.Context, collection *mongo.Collection, item T) (*mongo.UpdateResult, error) {
	return save(ctx, collection, item, true)
}

func SaveMany[T any](ctx context.Context, collection *mongo.Collection, items []T) (*mongo.BulkWriteResult, error) {
	mongoItems := make([]mongo.WriteModel, 0, len(items))

	for _, item := range items {
		updateOne := mongo.NewUpdateOneModel()
		filter, err := getPKFilter(item)
		if err != nil {
			return nil, err
		}

		updateOne.SetFilter(filter)
		updateOne.SetUpsert(true)
		updateOne.SetUpdate(bson.M{"$set": item})

		mongoItems = append(mongoItems, updateOne)
	}

	// If true, no writes will be executed after one fails.
	option := options.BulkWrite().SetOrdered(true)
	result, err := collection.BulkWrite(ctx, mongoItems, option)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	return result, nil
}

func Update[T any](ctx context.Context, collection *mongo.Collection, item T) (*mongo.UpdateResult, error) {
	return save(ctx, collection, item, false)
}

func Count(ctx context.Context, collection *mongo.Collection, filter bson.M) (int, error) {
	total, err := collection.CountDocuments(ctx, filter)
	return int(total), err
}

type fieldDetails struct {
	fieldName  string
	bsonName   string
	typeName   string
	fieldValue interface{}
}

func isNil[T any](arg T) bool {
	if v := reflect.ValueOf(arg); (v.Kind() == reflect.Ptr ||
		v.Kind() == reflect.Interface ||
		v.Kind() == reflect.Slice ||
		v.Kind() == reflect.Map ||
		v.Kind() == reflect.Chan ||
		v.Kind() == reflect.Func) && v.IsNil() {
		return true
	}
	return false
}
func getPKDetails[T any](item T) (details *fieldDetails, err error) {
	ptrVal := reflect.TypeOf(item)

	if ptrVal.Kind() == reflect.Ptr {
		ptrVal = ptrVal.Elem()
	}

	for i := 0; i < ptrVal.NumField(); i++ {
		field := ptrVal.Field(i)
		if field.Tag.Get("pk") == "true" {
			if field.Type.Kind() != reflect.String {
				// we expect the primary key field is string
				panicStr := fmt.Sprintf("type %s primary key field %s must be string", ptrVal.Name(), field.Name)
				panic(panicStr)
			}

			var fieldValue interface{} = nil
			if !isNil(item) {
				v := reflect.ValueOf(item)
				for v.Kind() == reflect.Ptr {
					v = v.Elem()
				}
				fieldValue = v.FieldByName(field.Name).Interface()
			}
			details = &fieldDetails{
				fieldName:  field.Name,
				bsonName:   field.Tag.Get("bson"),
				fieldValue: fieldValue,
				typeName:   ptrVal.Name(),
			}
			return details, nil
		}
	}

	return nil, core_error.StackError("no primary key found")
}

func getPKFilter[T any](item T) (filter bson.M, err error) {
	detail, err := getPKDetails(item)

	if err != nil {
		return nil, err
	}

	filter = bson.M{}
	filter[detail.bsonName] = detail.fieldValue

	return filter, nil
}

func save[T any](ctx context.Context, collection *mongo.Collection, item T, upsert bool) (*mongo.UpdateResult, error) {
	filter, err := getPKFilter(item)
	if err != nil {
		return nil, err
	}

	update := bson.M{
		"$set": item,
	}

	opt := options.Update().SetUpsert(upsert)

	result, err := collection.UpdateOne(ctx, filter, update, opt)
	if err != nil {
		return nil, core_error.StackError(err)
	}
	return result, nil
}

// ClassifyModifications takes two slices of entities of type T, orginalEntities and updateEntities,
// and returns three slices of entities of type T, create, update, and remove, and an error.
// It compares the two slices and returns the entities that were create, update, and remove.
// The function assumes that each entity has a primary key field that is a string.
// If an entity in updateEntities has an empty primary key,
// it is considered a new entity and is added to the create slice.
// If an entity in updateEntities has a non-empty primary key,
// it is considered an update entity and is added to the update slice.
// If an entity in orginalEntities is not found in updateEntities,
// it is considered a remove entity and is added to the remove slice.
func ClassifyModifications[T any](orginalEntities []T, updateEntities []T) (create []T, update []T, remove []T, err error) {

	create = make([]T, 0)
	update = make([]T, 0)
	remove = make([]T, 0)

	originEntityMap := make(map[interface{}]T)
	for _, entity := range orginalEntities {
		pkFieldDetail, err := getPKDetails[T](entity)
		if err != nil {
			return nil, nil, nil, err
		}
		key := pkFieldDetail.fieldValue

		originEntityMap[key] = entity
	}

	for _, entity := range updateEntities {
		pkFieldDetail, err := getPKDetails[T](entity)
		if err != nil {
			return nil, nil, nil, err
		}

		key := pkFieldDetail.fieldValue.(string)

		if key == "" {
			create = append(create, entity)
			continue
		}
		update = append(update, entity)
		delete(originEntityMap, key)
	}

	for _, entity := range originEntityMap {
		remove = append(remove, entity)
	}

	return create, update, remove, nil
}
