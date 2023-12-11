package command

import (
	"context"
	"errors"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/space/domain/error_code"
	"xrspace.io/server/modules/space/domain/repository"
)

type DeleteSpaceUseCase struct {
	repo repository.ISpaceRepository
}

var _ application.IUseCase = (*DeleteSpaceUseCase)(nil)

type DeleteSpaceCommand struct {
	SpaceId string `uri:"space_id" validate:"required"`
}

type DeleteSpaceResponse struct{}

func NewDeleteSpaceUseCase(repo repository.ISpaceRepository) *DeleteSpaceUseCase {
	return &DeleteSpaceUseCase{
		repo: repo,
	}
}

func (s *DeleteSpaceUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd, ok := cmdz.(*DeleteSpaceCommand)
	if !ok {
		panic("type error: expected an *DeleteSpaceCommand")
	}

	err := s.archive(ctx, cmd.SpaceId)

	if err != nil {
		return nil, core_error.NewCoreError(error_code.DeleteSpaceError, err)
	}

	return &DeleteSpaceResponse{}, nil
}

func (s *DeleteSpaceUseCase) archive(ctx context.Context, spaceId string) error {
	space, err := s.repo.FindById(ctx, spaceId)
	if err != nil {
		return err
	}
	if space == nil {
		return errors.New("space not found")
	}
	space.Archive()
	err = s.repo.Save(ctx, space)
	if err != nil {
		return err
	}
	return nil
}
