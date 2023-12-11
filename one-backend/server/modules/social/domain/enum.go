package domain

type FollowStatus int

const (
	Following FollowStatus = iota + 1
	Unfollowed
)
