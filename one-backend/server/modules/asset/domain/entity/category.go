package entity

import "xrspace.io/server/modules/asset/domain/entity/enum"

type CategoryItem struct {
	ID        string            `json:"id" bson:"id" pk:"true"`
	Type      enum.CategoryType `json:"type" bson:"type"`
	TitleI18n string            `json:"title_i18n" bson:"title_i18n"`
}

func (i *CategoryItem) Update(titleI18n string) {
	i.TitleI18n = titleI18n
}

func NewCategoryItem(ID string, Type enum.CategoryType, titleI18n string) *CategoryItem {
	return &CategoryItem{ID: ID, Type: Type, TitleI18n: titleI18n}
}
