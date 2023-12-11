package entity

import (
	"time"

	"github.com/google/uuid"
	"xrspace.io/server/modules/account/domain/value_object"
)

type AccountParams Account

type Account struct {
	CreatedAt              time.Time                     `bson:"created_at"`
	UpdatedAt              time.Time                     `bson:"updated_at"`
	IssuerResourceOwnerIDs value_object.ResourceOwnerIDs `bson:"issuer_resource_owner_ids"`
	ID                     string                        `bson:"id" pk:"true"`
	XrID                   string                        `bson:"xrid"`
	Username               string                        `bson:"username"`
	Nickname               string                        `bson:"nickname"`
	ResourceOwnerIDs       []string                      `bson:"resource_owner_ids"`
}

func NewAccount(params *AccountParams) *Account {
	if params.ID == "" {
		params.ID = uuid.NewString()
	}

	now := time.Now()

	if params.CreatedAt.IsZero() {
		params.CreatedAt = now
	}

	if params.UpdatedAt.IsZero() {
		params.UpdatedAt = now
	}

	resourceOwnerIDs := make([]string, 0, len(params.IssuerResourceOwnerIDs))
	for _, v := range params.IssuerResourceOwnerIDs {
		resourceOwnerIDs = append(resourceOwnerIDs, v)
	}

	return &Account{
		ID:                     params.ID,
		XrID:                   params.XrID,
		IssuerResourceOwnerIDs: params.IssuerResourceOwnerIDs,
		ResourceOwnerIDs:       resourceOwnerIDs,
		Username:               params.Username,
		Nickname:               params.Nickname,
		CreatedAt:              params.CreatedAt,
		UpdatedAt:              params.UpdatedAt,
	}
}
