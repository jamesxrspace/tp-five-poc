package query

import (
	"context"

	"github.com/go-playground/validator/v10"

	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/avatar/domain/avatar_errors"
	"xrspace.io/server/modules/avatar/domain/entity"
	"xrspace.io/server/modules/avatar/domain/repository"
	"xrspace.io/server/modules/avatar/domain/value_object"
)

type ListGroupAvatarsQuery struct {
	AppID value_object.AppID  `form:"-" validate:"required"`
	XrIDs []value_object.XrID `form:"xrids" validate:"required,min=1,max=50,unique"`
	pagination.PaginationQuery
}

func (q *ListGroupAvatarsQuery) Validate() error {
	validate := validator.New()
	return validate.Struct(q)
}

type ListGroupAvatarsResponse struct {
	Items []*entity.Avatar `json:"items"`
}

type ListGroupAvatarsUseCase struct {
	repository repository.IListGroupAvatarsRepository
}

func NewListGroupAvatarsUseCase(
	repository repository.IListGroupAvatarsRepository,
) *ListGroupAvatarsUseCase {
	return &ListGroupAvatarsUseCase{
		repository: repository,
	}
}

func (u *ListGroupAvatarsUseCase) Execute(ctx context.Context, q *ListGroupAvatarsQuery) (*ListGroupAvatarsResponse, error) {
	if err := q.Validate(); err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.ValidateError,
			err,
		)
	}

	playerResult, err := u.repository.ListAvatarPlayersByXrIDs(ctx, q.XrIDs, q.Offset, q.Size)
	if err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.ListAvatarPlayersError,
			err,
		)
	}

	avatarIDs := getAvatarIDsFromPlayers(playerResult.Items, q.AppID)
	avatarResult, err := u.repository.ListAvatarByAvatarIDs(ctx, avatarIDs, 0, 50)
	if err != nil {
		return nil, core_error.NewCoreError(
			avatar_errors.ListAvatarError,
			err,
		)
	}

	if len(avatarResult.Items) != len(q.XrIDs) {
		if err := u.setDefaultAvatars(ctx, q.AppID, q.XrIDs, avatarResult); err != nil {
			return nil, core_error.NewCoreError(
				avatar_errors.ListAvatarError,
				err,
			)
		}
	}

	return &ListGroupAvatarsResponse{
		Items: avatarResult.Items,
	}, nil
}

func (u *ListGroupAvatarsUseCase) setDefaultAvatars(ctx context.Context, appID value_object.AppID, xrIDs []value_object.XrID, avatarResult *repository.ListAvatarByAvatarIDsResult) error {
	defaultAvatar, err := u.repository.GetDefaultAvatar(ctx, appID)
	if err != nil {
		return err
	}

	resultMap := make(map[value_object.XrID]*entity.Avatar)
	for _, avatar := range avatarResult.Items {
		resultMap[avatar.XrID] = avatar
	}

	for _, xrid := range xrIDs {
		if _, ok := resultMap[xrid]; !ok {
			var defaultAvatar = *defaultAvatar
			defaultAvatar.XrID = xrid
			avatarResult.Items = append(avatarResult.Items, &defaultAvatar)
		}
	}

	return nil
}

func getAvatarIDsFromPlayers(players []*entity.AvatarPlayer, appID value_object.AppID) []value_object.AvatarID {
	avatarIDs := make([]value_object.AvatarID, 0, len(players))
	for _, player := range players {
		if _, ok := player.AvatarID[appID]; ok {
			avatarIDs = append(avatarIDs, player.AvatarID[appID])
		}
	}
	return avatarIDs
}
