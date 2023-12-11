package entity

import (
	"strings"

	"github.com/google/uuid"
	"xrspace.io/server/core/dependency/settings"
)

type Guest struct {
	Nickname string
	Email    string
	Password string
	Company  string
}

func NewGuest(config *settings.Guest, nickname string) (account *Guest) {
	emailPrefix := config.EmailPrefix
	emailSuffix := config.EmailSuffix
	guestCompany := config.Company

	email := func() string {
		randomString := strings.ToUpper(strings.ReplaceAll(uuid.NewString(), "-", "")[:12])

		return emailPrefix + "@" + string(randomString) + emailSuffix
	}()

	password := func() string {
		return uuid.NewString()[:12]
	}()

	guest := &Guest{
		Nickname: nickname,
		Company:  guestCompany,
		Email:    email,
		Password: password,
	}

	return guest
}
