package command

import (
	"context"
	"fmt"
	"time"

	"xrspace.io/server/core/arch/application"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/unit_of_work"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/error_code"
	"xrspace.io/server/modules/space/domain/repository"
	"xrspace.io/server/modules/space/domain/value_object"
)

type UpdateSpaceGroupUseCase struct {
	repo       repository.ISpaceGroupRepository
	spaceRepo  repository.ISpaceRepository
	unitOfWork unit_of_work.IUnitOfWork
}

var _ application.IUseCase = (*UpdateSpaceGroupUseCase)(nil)

type UpdateSpaceGroupCommand struct {
	SpaceGroupId string `uri:"space_group_id" validate:"required"`
	UpdateSpaceGroupBody
}

type UpdateSpaceGroupBody struct {
	Name        string                        `json:"name" validate:"required"`
	Status      value_object.SpaceGroupStatus `json:"status" validate:"required,oneof=enabled disabled"`
	StartAt     time.Time                     `json:"start_at"`
	EndAt       time.Time                     `json:"end_at"`
	Description string                        `json:"description"`
	Thumbnail   string                        `json:"thumbnail" validate:"omitempty,url"`
	SpaceIds    []string                      `json:"space_ids"`
}

type UpdateSpaceGroupResponse struct {
	entity.SpaceGroup
	Spaces []*SpaceResponse `json:"spaces"`
}

func NewUpdateSpaceGroupUseCase(repo repository.ISpaceGroupRepository, spaceRepo repository.ISpaceRepository, unitOfWork unit_of_work.IUnitOfWork) *UpdateSpaceGroupUseCase {
	return &UpdateSpaceGroupUseCase{
		repo:       repo,
		spaceRepo:  spaceRepo,
		unitOfWork: unitOfWork,
	}
}

func (s *UpdateSpaceGroupUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*UpdateSpaceGroupCommand)

	respz, err := s.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		return s.update(cx, *cmd)
	})

	if err != nil {
		return nil, core_error.NewCoreError(
			error_code.UpdateSpaceGroupError,
			err,
		)
	}

	resp := respz.(*UpdateSpaceGroupResponse)

	return resp, nil
}

func (s *UpdateSpaceGroupUseCase) update(ctx context.Context, cmd UpdateSpaceGroupCommand) (*UpdateSpaceGroupResponse, error) {
	spaceGroup, err := s.repo.FindById(ctx, cmd.SpaceGroupId)
	if err != nil {
		return nil, fmt.Errorf("find space group: %v", err)
	}
	if spaceGroup == nil {
		return nil, fmt.Errorf("space group not found")
	}

	spaceGroup, err = s.updateSpaceGroup(ctx, spaceGroup, cmd.UpdateSpaceGroupBody)
	if err != nil {
		return nil, fmt.Errorf("update space group: %v", err)
	}
	space, err := s.processSpaces(ctx, spaceGroup, cmd.SpaceIds)
	if err != nil {
		return nil, fmt.Errorf("process spaces: %v", err)
	}

	return &UpdateSpaceGroupResponse{
		SpaceGroup: *spaceGroup,
		Spaces:     s.generateSpaceResponse(space),
	}, nil
}

func (s *UpdateSpaceGroupUseCase) updateSpaceGroup(ctx context.Context, spaceGroup *entity.SpaceGroup, cmd UpdateSpaceGroupBody) (*entity.SpaceGroup, error) {
	spaceGroup.Name = cmd.Name
	spaceGroup.Status = cmd.Status
	spaceGroup.StartAt = cmd.StartAt
	spaceGroup.EndAt = cmd.EndAt
	spaceGroup.Description = cmd.Description
	spaceGroup.Thumbnail = cmd.Thumbnail

	err := s.repo.Save(ctx, spaceGroup)

	if err != nil {
		return nil, err
	}

	return spaceGroup, nil
}

func (s *UpdateSpaceGroupUseCase) processSpaces(ctx context.Context, spaceGroup *entity.SpaceGroup, spaceIds []string) ([]*entity.Space, error) {
	updated, err := processSpaceGroupSpaces(ctx, spaceGroup, spaceIds, s.spaceRepo)
	if err != nil {
		return nil, err
	}

	return updated, nil
}

func (s *UpdateSpaceGroupUseCase) generateSpaceResponse(spaces []*entity.Space) []*SpaceResponse {
	var spaceResponses []*SpaceResponse
	for _, space := range spaces {
		spaceResponses = append(spaceResponses, &SpaceResponse{
			SpaceId: space.SpaceId,
			Name:    space.Name,
		})
	}
	return spaceResponses
}
