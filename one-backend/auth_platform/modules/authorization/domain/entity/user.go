package entity

import (
	"time"

	"golang.org/x/crypto/bcrypt"
)

type User struct {
	CreatedAt     time.Time
	UpdatedAt     time.Time
	ID            string
	Username      string
	Nickname      string
	Password      string
	Email         string
	Company       string
	AccessToken   string
	RefreshToken  string
	EmailVerified bool
}

func NewUser(userID, username, nickname, password, email, company string, emailVerified bool, cost int) *User {
	return &User{
		ID:            userID,
		Username:      username,
		Nickname:      nickname,
		Password:      encrypt(password, cost),
		Email:         email,
		EmailVerified: emailVerified,
		Company:       company,
		CreatedAt:     time.Now(),
		UpdatedAt:     time.Now(),
	}
}

func encrypt(password string, cost int) string {
	encrypt, err := bcrypt.GenerateFromPassword([]byte(password), cost)
	if err != nil {
		panic(err)
	}
	return string(encrypt)
}

func (u *User) CheckPassword(encrypt, password string) error {
	err := bcrypt.CompareHashAndPassword([]byte(encrypt), []byte(password))
	if err != nil {
		return err
	}
	return nil
}
