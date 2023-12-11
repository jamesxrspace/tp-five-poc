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

type UpdateSpaceUseCase struct {
	repo           repository.ISpaceRepository
	spaceGroupRepo repository.ISpaceGroupRepository
	unitOfWork     unit_of_work.IUnitOfWork
}

var _ application.IUseCase = (*UpdateSpaceUseCase)(nil)

type UpdateSpaceCommand struct {
	SpaceId string `uri:"space_id" validate:"required"`
	UpdateSpaceBody
}

type UpdateSpaceBody struct {
	SpaceGroupId string    `json:"space_group_id"`
	Name         string    `json:"name" validate:"required"`
	Description  string    `json:"description"`
	Thumbnail    string    `json:"thumbnail" validate:"omitempty,url"`
	Addressable  string    `json:"addressable"`
	StartAt      time.Time `json:"start_at"`
	EndAt        time.Time `json:"end_at"`
}

type UpdateSpaceResponse struct {
	entity.Space
}

func NewUpdateSpaceUseCase(repo repository.ISpaceRepository, spaceGroupRepo repository.ISpaceGroupRepository, unitOfWork unit_of_work.IUnitOfWork) *UpdateSpaceUseCase {
	return &UpdateSpaceUseCase{
		repo:           repo,
		spaceGroupRepo: spaceGroupRepo,
		unitOfWork:     unitOfWork,
	}
}

func (s *UpdateSpaceUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*UpdateSpaceCommand)

	result, err := s.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		return s.update(cx, *cmd)
	})

	if err != nil {
		return nil, err
	}

	spaceResp := result.(*UpdateSpaceResponse)

	return spaceResp, nil
}

func (s *UpdateSpaceUseCase) update(ctx context.Context, cmd UpdateSpaceCommand) (*UpdateSpaceResponse, error) {
	space, err := s.repo.FindById(ctx, cmd.SpaceId)
	if err != nil {
		return nil, err
	}
	if space == nil {
		return nil, core_error.NewEntityNotFoundError("space", cmd.SpaceId)
	}

	if cmd.SpaceGroupId != "" {
		spaceGroup, err := s.spaceGroupRepo.FindById(ctx, cmd.SpaceGroupId)
		if err != nil {
			return nil, err
		}
		if spaceGroup == nil {
			return nil, core_error.NewEntityNotFoundError("space group", cmd.SpaceGroupId)
		}
	}

	space, err = s.updateSpace(ctx, space, cmd.UpdateSpaceBody)
	if err != nil {
		return nil, err
	}

	spaceResp := &UpdateSpaceResponse{
		Space: *space,
	}

	return spaceResp, nil
}

func (s *UpdateSpaceUseCase) updateSpace(ctx context.Context, space *entity.Space, cmd UpdateSpaceBody) (*entity.Space, error) {
	space.SpaceGroupId = cmd.SpaceGroupId
	space.Name = cmd.Name
	space.StartAt = cmd.StartAt
	space.EndAt = cmd.EndAt
	space.Description = cmd.Description
	space.Thumbnail = cmd.Thumbnail
	space.Addressable = cmd.Addressable
	err := s.repo.Save(ctx, space)

	if err != nil {
		return nil, err
	}

	return space, nil
}
