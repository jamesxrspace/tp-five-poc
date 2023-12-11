package command

import (
	"context"
	"errors"
	"fmt"

	mongoUtil "xrspace.io/server/core/arch/port/repository/mongo"
	"xrspace.io/server/modules/space/domain/entity"
	"xrspace.io/server/modules/space/domain/repository"
)

func processSpaceGroupSpaces(ctx context.Context, spaceGroup *entity.SpaceGroup, spaceIds []string, spaceRepo repository.ISpaceRepository) ([]*entity.Space, error) {
	originSpaces, err := spaceRepo.FindBySpaceGroupId(ctx, spaceGroup.SpaceGroupId)
	if err != nil {
		return nil, err
	}

	spaces, err := spaceRepo.FindByIds(ctx, spaceIds)
	if err != nil {
		return nil, err
	}
	if len(spaces) != len(spaceIds) {
		return nil, errors.New("part of space not found")
	}

	_, updated, deleted, err := mongoUtil.ClassifyModifications[*entity.Space](originSpaces, spaces)
	if err != nil {
		return nil, fmt.Errorf("process space list diff: %v", err)
	}

	for _, space := range updated {
		space.SpaceGroupId = spaceGroup.SpaceGroupId
	}
	for _, space := range deleted {
		space.SpaceGroupId = ""
	}
	saveSpaces := append(updated, deleted...)

	if len(saveSpaces) > 0 {
		err = spaceRepo.SaveMany(ctx, saveSpaces)
		if err != nil {
			return nil, fmt.Errorf("save space: %v", err)
		}
	}

	return updated, nil
}
