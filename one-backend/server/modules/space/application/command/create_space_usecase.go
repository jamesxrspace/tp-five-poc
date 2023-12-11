package command

import (
	"context"
	"time"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/repository"
)

type CreateSpaceUseCase struct {
	repo           repository.ISpaceRepository
	spaceGroupRepo repository.ISpaceGroupRepository
	unitOfWork     unit_of_work.IUnitOfWork
}

var _ application.IUseCase = (*CreateSpaceUseCase)(nil)

type CreateSpaceCommand struct {
	SpaceGroupId string    `json:"space_group_id"`
	Name         string    `json:"name" validate:"required"`
	Description  string    `json:"description"`
	Thumbnail    string    `json:"thumbnail" validate:"omitempty,url"`
	Addressable  string    `json:"addressable"`
	StartAt      time.Time `json:"start_at"`
	EndAt        time.Time `json:"end_at"`
}

type CreateSpaceResponse struct {
	entity.Space
}

func NewCreateSpaceUseCase(repo repository.ISpaceRepository, spaceGroupRepo repository.ISpaceGroupRepository, unitOfWork unit_of_work.IUnitOfWork) *CreateSpaceUseCase {
	return &CreateSpaceUseCase{
		repo:           repo,
		spaceGroupRepo: spaceGroupRepo,
		unitOfWork:     unitOfWork,
	}
}

func (s *CreateSpaceUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*CreateSpaceCommand)

	resp, err := s.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		return s.create(cx, *cmd)
	})

	if err != nil {
		return nil, err
	}

	spaceResp := resp.(*CreateSpaceResponse)

	return spaceResp, nil
}

func (s *CreateSpaceUseCase) create(ctx context.Context, cmd CreateSpaceCommand) (*CreateSpaceResponse, error) {
	if cmd.SpaceGroupId != "" {
		spaceGroup, err := s.spaceGroupRepo.FindById(ctx, cmd.SpaceGroupId)
		if err != nil {
			return nil, err
		}
		if spaceGroup == nil {
			return nil, core_error.NewEntityNotFoundError("space group", cmd.SpaceGroupId)
		}
	}

	spaceEntity := entity.NewSpace(cmd.SpaceGroupId, cmd.Name, cmd.Description, cmd.StartAt, cmd.EndAt, cmd.Thumbnail, cmd.Addressable)

	spaceEntity, err := s.repo.Create(ctx, spaceEntity)

	if err != nil {
		return nil, err
	}

	return &CreateSpaceResponse{
		Space: *spaceEntity}, nil
}
