package repository

import (
	"xrspace.io/server/core/arch/domain/event"
)

type IMetricRepository interface {
	Inc(topic event.Topic, labels map[string]string)
}
