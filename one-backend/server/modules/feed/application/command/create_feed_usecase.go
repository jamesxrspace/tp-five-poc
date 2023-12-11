package command

import (
	"context"
	"strings"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
)

var _ application.IUseCase = (*CreateFeedUseCase)(nil)

type CreateFeedCommand struct {
	XrID         string   `json:"xrid" validate:"required" token:"xrid"`
	Type         string   `json:"type" validate:"required,oneof=avatar_reel space avatar_text avatar_img avatar"`
	RefID        string   `json:"ref_id" validate:"required"`
	ThumbnailUrl string   `json:"thumbnail_url" validate:"required,url"`
	Categories   []string `json:"categories" validate:"required,min=1"`
}

type CreateFeedResponse struct {
	Feed *entity.Feed `json:"feed"`
}

type CreateFeedUseCase struct {
	dep define.Dependency
}

func NewCreateFeedUseCase(dep define.Dependency) *CreateFeedUseCase {
	return &CreateFeedUseCase{
		dep: dep,
	}
}

func (u *CreateFeedUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*CreateFeedCommand)

	err := u.validateCategories(ctx, cmd.Categories)
	if err != nil {
		return nil, core_error.NewCodeError(core_error.ValidationErrCode, err)
	}

	feed, err := u.dep.QueryRepo.GetFeedByRef(ctx, cmd.Type, cmd.RefID)
	if err != nil {
		return nil, err
	}

	if feed != nil {
		return nil, core_error.StackError("feed already exists")
	}

	feed = entity.NewFeed(&entity.FeedParams{
		XrID:         cmd.XrID,
		Type:         cmd.Type,
		RefID:        cmd.RefID,
		ThumbnailUrl: cmd.ThumbnailUrl,
		Categories:   cmd.Categories,
	})

	if err := u.dep.FeedRepo.Save(ctx, feed); err != nil {
		return nil, err
	}

	return &CreateFeedResponse{
		Feed: feed,
	}, nil
}

var _ application.IEventCommand = (*CreateFeedCommand)(nil)

func (c *CreateFeedCommand) SetEvent(e *event.Event) error {
	categories := strings.Split(e.Payload["categories"], ",")

	c.RefID = e.Payload["ref_id"]
	c.Type = e.Payload["type"]
	c.XrID = e.Payload["xrid"]
	c.Categories = categories
	c.ThumbnailUrl = e.Payload["thumbnail_url"]

	return nil
}

func (u *CreateFeedUseCase) validateCategories(ctx context.Context, categories []string) error {
	result, err := u.dep.QueryRepo.ListFeedCategory(ctx, &define.ListFeedCategoryFilter{
		Type:       enum.CateFeed,
		TitleI18ns: categories,
	})

	if len(result) == 0 {
		return core_error.StackError("feed category not found")
	}

	return err
}
