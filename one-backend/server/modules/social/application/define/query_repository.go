package define

import (
	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/social/domain"
)

type ListFollowFilter struct {
	XrID string
	pagination.PaginationQuery
}

type ListFollowResult struct {
	Items []string `bson:"items"`
	Total int      `bson:"total"`
}

type IQueryRepository interface {
	GetFollow(xrId, followingId string) (*domain.Follow, error)
	GetFollowersCount(xrId string) (int, error)
	GetFollowingCount(xrId string) (int, error)
	ListFollowers(filter *ListFollowFilter) (*ListFollowResult, error)
	ListFollowing(filter *ListFollowFilter) (*ListFollowResult, error)
	IsUserExist(xrId string) (bool, error)
}
