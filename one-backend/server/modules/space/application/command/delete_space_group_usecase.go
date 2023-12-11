package command

import (
	"context"
	"errors"
	"fmt"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/space/domain/error_code"
	"xrspace.io/server/modules/space/domain/repository"
)

type DeleteSpaceGroupUseCase struct {
	repo       repository.ISpaceGroupRepository
	spaceRepo  repository.ISpaceRepository
	unitOfWork unit_of_work.IUnitOfWork
}

var _ application.IUseCase = (*DeleteSpaceGroupUseCase)(nil)

type DeleteSpaceGroupCommand struct {
	SpaceGroupId string `uri:"space_group_id" validate:"required"`
}

type DeleteSpaceGroupResponse struct{}

func NewDeleteSpaceGroupUseCase(repo repository.ISpaceGroupRepository, spaceRepo repository.ISpaceRepository, unitOfWork unit_of_work.IUnitOfWork) *DeleteSpaceGroupUseCase {
	return &DeleteSpaceGroupUseCase{
		repo:       repo,
		spaceRepo:  spaceRepo,
		unitOfWork: unitOfWork,
	}
}

func (s *DeleteSpaceGroupUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*DeleteSpaceGroupCommand)

	_, err := s.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		return nil, s.archive(cx, cmd.SpaceGroupId)
	})

	if err != nil {
		return nil, core_error.NewCoreError(
			error_code.DeleteSpaceGroupError,
			err,
		)
	}
	return &DeleteSpaceGroupResponse{}, nil
}

func (s *DeleteSpaceGroupUseCase) archive(ctx context.Context, spaceGroupId string) error {
	spaceGroup, err := s.repo.FindById(ctx, spaceGroupId)
	if err != nil {
		return err
	}
	if spaceGroup == nil {
		return errors.New("space group not found")
	}
	spaceGroup.Archive()
	err = s.repo.Save(ctx, spaceGroup)
	if err != nil {
		return err
	}
	// archive all spaces in this group
	spaces, err := s.spaceRepo.FindBySpaceGroupId(ctx, spaceGroupId)
	if err != nil {
		return fmt.Errorf("find spaces by space group id: %v", err)
	}
	for _, space := range spaces {
		space.Archive()
		err = s.spaceRepo.Save(ctx, space)
		if err != nil {
			return fmt.Errorf("save space: %v", err)
		}
	}
	return nil
}
