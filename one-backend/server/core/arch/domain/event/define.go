package event

import (
	"time"

	"github.com/google/uuid"
)

const (
	ReelPublishedEvent Topic = "reel_published"
)

type Event struct {
	// ID is the unique identifier of the event.
	ID ID `json:"id"`
	// Type is the type of the event.
	Topic Topic `json:"topic"`
	// Version is the version of the event.
	Version string `json:"version"`
	// Timestamp is the time when the event occurred.
	Timestamp time.Time `json:"timestamp"`
	// Payload is the data of the event.
	Payload map[string]string `json:"payload"`
	Module  string            `json:"module"`
}

type Topic string
type ID string

// New creates a new event.
func New(topic Topic, module string, version string, payload map[string]string) *Event {
	timestamp := time.Now()
	return &Event{
		ID:        newID(),
		Topic:     topic,
		Module:    module,
		Version:   version,
		Timestamp: timestamp,
		Payload:   payload,
	}
}

func newID() ID {
	id, _ := uuid.NewUUID()
	return ID(id.String())
}

func (e *Event) ToMap() map[string]string {
	return map[string]string{
		"id":        string(e.ID),
		"topic":     string(e.Topic),
		"version":   e.Version,
		"module":    e.Module,
		"timestamp": e.Timestamp.Format(time.RFC3339Nano),
	}
}
