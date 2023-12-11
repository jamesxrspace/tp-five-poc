package command

import (
	"context"
	"fmt"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/feed/application/define"
)

var _ application.IUseCase = (*DeleteReelUseCase)(nil)

type DeleteReelCommand struct {
	XrID   string `json:"xrid" validate:"required" token:"xrid"`
	ReelID string `uri:"reel_id" json:"reel_id" validate:"required"`
}

type DeleteReelResponse struct {
}

type DeleteReelUseCase struct {
	dep define.Dependency
}

func NewDeleteReelUseCase(dep define.Dependency) *DeleteReelUseCase {
	return &DeleteReelUseCase{
		dep: dep,
	}
}

func (u *DeleteReelUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*DeleteReelCommand)

	result, err := u.dep.QueryRepo.ListReel(ctx, &define.ListReelFilter{
		ReelID: cmd.ReelID,
		PaginationQuery: pagination.PaginationQuery{
			Size: 1,
		},
	})
	if err != nil {
		return nil, err
	}
	if len(result.Items) == 0 {
		return nil, core_error.StackError(fmt.Sprintf("reel not found, reel_id: %s", cmd.ReelID))
	}

	reel := result.Items[0]
	if reel.XrID != cmd.XrID {
		return nil, core_error.NewCodeError(core_error.PermissionErrCode, fmt.Errorf("not allowed to delete reel, reel_id: %s", cmd.ReelID))
	}

	reel.Delete()

	if err := u.dep.ReelRepo.Save(ctx, reel); err != nil {
		return nil, err
	}

	return &DeleteReelResponse{}, nil
}
