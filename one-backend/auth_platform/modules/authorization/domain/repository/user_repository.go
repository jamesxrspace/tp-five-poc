package repository

import "auth_platform/modules/authorization/domain/entity"

type IUserRepository interface {
	FindUserByEmail(email string) (*entity.User, error)
	FindUserByID(userID string) (*entity.User, error)
	FindUserByRefreshToken(token string) (*entity.User, error)
	Save(user *entity.User) error
}
