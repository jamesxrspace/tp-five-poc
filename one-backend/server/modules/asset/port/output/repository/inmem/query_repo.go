package inmem

import (
	"context"

	"xrspace.io/server/modules/asset/application/query"
)

var _ query.IQueryRepository = (*QueryRepository)(nil)

type QueryRepository struct{}

func (q *QueryRepository) QueryCategory(ctx context.Context, offset int, size int) ([]*query.CategoryItem, query.Total, error) {
	items := []*query.CategoryItem{
		{
			ID:        "001",
			TitleI18n: "decoration.category.recent",
		},
		{
			ID:        "002",
			TitleI18n: "decoration.category.animal",
		},
		{
			ID:        "003",
			TitleI18n: "decoration.category.fruit",
		},
		{
			ID:        "004",
			TitleI18n: "decoration.category.furniture",
		},
		{
			ID:        "005",
			TitleI18n: "decoration.category.label",
		},
		{
			ID:        "006",
			TitleI18n: "decoration.category.anaimal",
		},
	}

	return items, query.Total(len(items)), nil
}

func (q *QueryRepository) QueryDecorations(ctx context.Context, tags []string, offset, size int) ([]*query.DecorationItem, query.Total, error) {
	items := []*query.DecorationItem{
		{
			Id:            "1",
			TitleI18n:     "desk_01",
			BundleID:      "4ccf31da-9288-4e1d-9ec2-b41aa6092b27",
			DecorationKey: "Scene Object/Prefabs/SM_Generic_Tree_03.prefab",
			ThumbnailKey:  "Scene Object/Thumbnails/Icon 01.png",
			CategoryID:    []string{"001"},
			Tags:          []string{"decoration.category.Recent", "decoration.category.Animal"},
		},
		{
			Id:            "2",
			TitleI18n:     "desk_02",
			BundleID:      "51882800-1175-4179-96cc-bd096eb7421b",
			DecorationKey: "Scene Object/Prefabs/SM_PolygonPrototype_Icon_Coin_01.prefab",
			ThumbnailKey:  "Scene Object/Thumbnails/Icon 02.png",
			CategoryID:    []string{"001", "002"},
			Tags:          []string{"decoration.category.Fruit", "decoration.category.Furniture"},
		},
		{
			Id:            "3",
			TitleI18n:     "cat_03",
			BundleID:      "2355e8e4-05ca-4d76-9573-80c6286bfd62",
			DecorationKey: "Scene Object/Prefabs/SM_PolygonPrototype_Prop_Crate_03.prefab",
			ThumbnailKey:  "Scene Object/Thumbnails/Icon 03.png",
			CategoryID:    []string{"002", "003"},
			Tags:          []string{"Fruit", "Animal"},
		},
		{
			Id:            "4",
			TitleI18n:     "avatar_01",
			BundleID:      "c6792315-904f-4416-b592-c8a88e6d3f12",
			DecorationKey: "Scene Object/Prefabs/avatar.prefab",
			ThumbnailKey:  "Scene Object/Thumbnails/avatar.png",
			CategoryID:    []string{"001"},
			Tags:          []string{"avatar"},
		},
	}

	return items, 3, nil
}
