package enum

type LikeStatus string
type TargetType string

const (
	LikeStatusLiked   LikeStatus = "liked"
	LikeStatusUnliked LikeStatus = "unliked"
	TargetTypeFeed    TargetType = "feed"
)
