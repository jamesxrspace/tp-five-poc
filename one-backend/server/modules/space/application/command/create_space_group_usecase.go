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
)

type CreateSpaceGroupUseCase struct {
	repo       repository.ISpaceGroupRepository
	spaceRepo  repository.ISpaceRepository
	unitOfWork unit_of_work.IUnitOfWork
}

var _ application.IUseCase = (*CreateSpaceGroupUseCase)(nil)

type CreateSpaceGroupCommand struct {
	Name        string    `json:"name" validate:"required"`
	StartAt     time.Time `json:"start_at"`
	EndAt       time.Time `json:"end_at"`
	Description string    `json:"description"`
	Thumbnail   string    `json:"thumbnail" validate:"omitempty,url"`
	SpaceIds    []string  `json:"space_ids"`
}

type SpaceResponse struct {
	SpaceId string `json:"space_id"`
	Name    string `json:"name"`
}

type CreateSpaceGroupResponse struct {
	entity.SpaceGroup
	Spaces []*SpaceResponse `json:"spaces"`
}

func NewCreateSpaceGroupUseCase(repo repository.ISpaceGroupRepository, spaceRepo repository.ISpaceRepository, unitOfWork unit_of_work.IUnitOfWork) *CreateSpaceGroupUseCase {
	return &CreateSpaceGroupUseCase{
		repo:       repo,
		spaceRepo:  spaceRepo,
		unitOfWork: unitOfWork,
	}
}

func (s *CreateSpaceGroupUseCase) Execute(ctx context.Context, cmdz any) (any, error) {
	cmd := cmdz.(*CreateSpaceGroupCommand)

	respz, err := s.unitOfWork.WithTransaction(ctx, func(cx context.Context) (any, error) {
		return s.create(cx, *cmd)
	})

	if err != nil {
		return nil, core_error.NewCoreError(
			error_code.CreateSpaceGroupError,
			err,
		)
	}

	resp := respz.(*CreateSpaceGroupResponse)

	return resp, nil
}

func (s *CreateSpaceGroupUseCase) create(ctx context.Context, cmd CreateSpaceGroupCommand) (*CreateSpaceGroupResponse, error) {
	spaceGroup, err := s.createSpaceGroup(ctx, cmd)
	if err != nil {
		return nil, fmt.Errorf("create space group: %v", err)
	}

	spaces, err := s.processSpace(ctx, spaceGroup, cmd.SpaceIds)
	if err != nil {
		return nil, fmt.Errorf("update space: %v", err)
	}
	return &CreateSpaceGroupResponse{
		SpaceGroup: *spaceGroup,
		Spaces:     s.generateSpaceResponse(spaces),
	}, nil
}

func (s *CreateSpaceGroupUseCase) createSpaceGroup(ctx context.Context, cmd CreateSpaceGroupCommand) (*entity.SpaceGroup, error) {
	group := entity.NewSpaceGroup(cmd.Name, cmd.StartAt, cmd.EndAt, cmd.Description, cmd.Thumbnail)
	return s.repo.Create(ctx, group)
}

func (s *CreateSpaceGroupUseCase) processSpace(ctx context.Context, spaceGroup *entity.SpaceGroup, spaceIds []string) ([]*entity.Space, error) {
	updated, err := processSpaceGroupSpaces(ctx, spaceGroup, spaceIds, s.spaceRepo)
	if err != nil {
		return nil, err
	}

	return updated, nil
}

func (s *CreateSpaceGroupUseCase) generateSpaceResponse(spaces []*entity.Space) []*SpaceResponse {
	var spaceResponses []*SpaceResponse
	for _, space := range spaces {
		spaceResponses = append(spaceResponses, &SpaceResponse{
			SpaceId: space.SpaceId,
			Name:    space.Name,
		})
	}
	return spaceResponses
}
