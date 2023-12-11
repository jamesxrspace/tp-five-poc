package event

import (
	"strings"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/modules/reel/domain/enum"
)

func NewReelPublishedEvent(reelID string, xrID string, thumbnailUrl string, categories []string) *event.Event {
	return event.New(
		event.ReelPublishedEvent,
		moduleName,
		eventVersion,
		map[string]string{
			"ref_id":        reelID,
			"type":          enum.FeedTypeReel,
			"xrid":          xrID,
			"thumbnail_url": thumbnailUrl,
			"categories":    strings.Join(categories, ","),
		},
	)
}
