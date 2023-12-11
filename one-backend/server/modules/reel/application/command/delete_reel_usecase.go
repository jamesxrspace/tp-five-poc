package command

import (
	"context"
	"fmt"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/reel/application/define"
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
		return nil, core_error.NewEntityNotFoundError("reel", cmd.ReelID)
	}

	if len(result.Items) != 1 {
		return nil, core_error.StackError(fmt.Errorf("can not match reel, reel_id: %s , match count: %d", cmd.ReelID, len(result.Items)))
	}

	reel := result.Items[0]
	if reel.XrID != cmd.XrID {
		return nil, core_error.NewCodeError(core_error.PermissionErrCode, fmt.Errorf("not allowed to delete reel, xrid: %s , reel_id: %s", cmd.XrID, cmd.ReelID))
	}

	if reel.IsDelete() {
		return &DeleteReelResponse{}, nil
	}

	reel.Delete()

	if err := u.dep.ReelRepo.Save(ctx, reel); err != nil {
		return nil, err
	}

	return &DeleteReelResponse{}, nil
}
