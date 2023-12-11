package inmem

import (
	"context"
	"errors"
	"fmt"
	"reflect"
)

type GenericInMemRepository[T any] struct {
	storage map[interface{}]T
}

func NewInMemGenericRepository[T any](storage map[interface{}]T) *GenericInMemRepository[T] {
	if storage == nil {
		storage = make(map[interface{}]T)
	}

	return &GenericInMemRepository[T]{
		storage: storage,
	}
}

func getPKValue[T any](item T) string {
	ptrVal := reflect.TypeOf(item)

	if ptrVal.Kind() == reflect.Ptr {
		ptrVal = ptrVal.Elem()
	}

	for i := 0; i < ptrVal.NumField(); i++ {
		field := ptrVal.Field(i)
		if field.Tag.Get("pk") == "true" {
			fieldValue := reflect.ValueOf(item).Elem().FieldByName(field.Name).Interface()
			return fieldValue.(string)
		}
	}

	panic(fmt.Sprintf("%s has no pk field defined", ptrVal.Name()))
}

func (g *GenericInMemRepository[T]) Save(_ context.Context, item T) error {
	g.storage[getPKValue(item)] = item
	return nil
}

func (g *GenericInMemRepository[T]) Get(_ context.Context, id string) (result T, err error) {
	// 當 T 不是 pointer type 的時後 `var result T` 會建立一個 instance
	// 在 FindByID 的設計，在 no document 的時後，會希望回傳 nil, nil
	// 這個時後如果不是 pointer type 的時後，會無法達成
	if reflect.TypeOf(result).Kind() != reflect.Ptr {
		return result, errors.New("type must be a pointer")
	}

	if obj, ok := g.storage[id]; ok {
		return obj, nil
	}

	return result, nil
}
