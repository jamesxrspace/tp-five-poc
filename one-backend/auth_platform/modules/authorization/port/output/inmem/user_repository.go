package inmem

import (
	"auth_platform/core/util"
	"auth_platform/modules/authorization/domain/entity"
	"auth_platform/modules/authorization/domain/repository"
	"errors"
)

type UserRepository struct {
	Users map[string]*entity.User
}

var _ repository.IUserRepository = (*UserRepository)(nil)

func NewUserRepository() *UserRepository {
	return &UserRepository{
		Users: util.GenFakeUsers(10, false),
	}
}

func (r *UserRepository) FindUserByEmail(email string) (*entity.User, error) {
	for _, user := range r.Users {
		if user.Email == email {
			return user, nil
		}
	}

	return nil, errors.New("user not found")
}

func (r *UserRepository) FindUserByID(userID string) (*entity.User, error) {
	for _, user := range r.Users {
		if user.ID == userID {
			return user, nil
		}
	}

	return nil, errors.New("user not found")
}

func (r *UserRepository) FindUserByRefreshToken(token string) (*entity.User, error) {
	for _, user := range r.Users {
		if user.RefreshToken == token {
			return user, nil
		}
	}

	return nil, errors.New("user not found")
}

func (r *UserRepository) Save(user *entity.User) error {
	r.Users[user.ID] = user
	return nil
}
