package command

import (
	"context"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	assetEnum "xrspace.io/server/modules/asset/domain/entity/enum"
	"xrspace.io/server/modules/reel/application/define"
	"xrspace.io/server/modules/reel/domain/entity"
	"xrspace.io/server/modules/reel/domain/enum"
)

var _ application.IUseCase = (*CreateReelUseCase)(nil)

type CreateReelCommand struct {
	Description      string   `json:"description"`
	Thumbnail        string   `json:"thumbnail" validate:"required,url"`
	Video            string   `json:"video" validate:"required,url"`
	Xrs              string   `json:"xrs" validate:"required,url"`
	XrID             string   `json:"xrid" validate:"required" token:"xrid"`
	ParentReelID     string   `json:"parent_reel_id"`
	JoinMode         string   `json:"join_mode" validate:"required,oneof=off all friends_followers me"`
	Categories       []string `json:"categories" validate:"required,min=1"`
	MusicToMotionUrl string   `json:"music_to_motion_url" validate:"omitempty,url"`
}

func (c *CreateReelCommand) hasParentReelID() bool {
	return c.ParentReelID != ""
}

type CreateReelResponse struct {
	Reel *entity.Reel
}

type CreateReelUseCase struct {
	dep define.Dependency
}

func NewCreateReelUseCase(dep define.Dependency) *CreateReelUseCase {
	return &CreateReelUseCase{
		dep: dep,
	}
}

func (u *CreateReelUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*CreateReelCommand)

	err := u.validateCategories(ctx, cmd.Categories)
	if err != nil {
		return nil, core_error.NewCodeError(core_error.ValidationErrCode, err)
	}

	reel := entity.NewReel(&entity.ReelParams{
		Description:      cmd.Description,
		Thumbnail:        cmd.Thumbnail,
		Video:            cmd.Video,
		Xrs:              cmd.Xrs,
		XrID:             cmd.XrID,
		Categories:       cmd.Categories,
		JoinMode:         cmd.JoinMode,
		MusicToMotionUrl: cmd.MusicToMotionUrl,
	})

	if cmd.hasParentReelID() {
		parentReel, err := u.getPublishedParentReel(ctx, cmd.ParentReelID)
		if err != nil {
			return nil, err
		}
		if parentReel == nil {
			return nil, core_error.NewEntityNotFoundError("parent reel", cmd.ParentReelID)
		}
		parentReelIDs := parentReel.ParentReelIDs
		reel.ParentReelIDs = append(parentReelIDs, parentReel.ID)
		reel.RootReelID = parentReel.RootReelID
	} else {
		reel.RootReelID = reel.ID
	}

	if err := u.dep.ReelRepo.Save(ctx, reel); err != nil {
		return nil, err
	}

	return &CreateReelResponse{
		Reel: reel,
	}, nil
}

func (u *CreateReelUseCase) getPublishedParentReel(ctx context.Context, parentReelID string) (*entity.Reel, error) {
	result, err := u.dep.QueryRepo.ListReel(ctx, &define.ListReelFilter{
		ReelID: parentReelID,
		Status: enum.ReelStatusPublished,
		PaginationQuery: pagination.PaginationQuery{
			Size: 1,
		},
	})
	if err != nil {
		return nil, err
	}
	if len(result.Items) == 0 {
		return nil, nil
	}
	return result.Items[0], nil
}

func (u *CreateReelUseCase) validateCategories(ctx context.Context, categories []string) error {
	result, err := u.dep.QueryRepo.ListFeedCategory(ctx, &define.ListFeedCategoryFilter{
		Type:       assetEnum.CateFeed,
		TitleI18ns: categories,
	})

	if len(result) == 0 {
		return core_error.StackError("feed category not found")
	}

	return err
}
