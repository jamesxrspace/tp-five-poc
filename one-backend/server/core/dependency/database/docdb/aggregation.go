package docdb

import (
	"go.mongodb.org/mongo-driver/bson"
)

func GetPaginateAggregation(skip int, limit int, match bson.M, sort bson.D, project bson.D, lookup bson.D) bson.A {
	items := bson.A{}
	items = append(items,
		bson.D{
			{Key: "$skip", Value: skip},
		},
		bson.D{{Key: "$limit", Value: limit}},
	)
	if lookup != nil {
		items = append(items, bson.D{
			{Key: "$lookup", Value: lookup},
		})
	}
	items = append(items, bson.D{
		{Key: "$project", Value: project},
	})

	return bson.A{
		bson.D{
			{Key: "$match", Value: match},
		},
		bson.D{
			{Key: "$sort", Value: sort},
		},
		bson.D{
			{Key: "$facet", Value: bson.D{
				{Key: "total", Value: bson.A{
					bson.D{
						{Key: "$count", Value: "count"},
					},
				}},
				{Key: "items", Value: items},
			}},
		},
		bson.D{
			{Key: "$unwind", Value: bson.D{
				{Key: "path", Value: "$total"},
				{Key: "preserveNullAndEmptyArrays", Value: false},
			}},
		},
		bson.D{
			{Key: "$project", Value: bson.D{
				{Key: "total", Value: "$total.count"},
				{Key: "items", Value: 1},
			}},
		},
	}
}
