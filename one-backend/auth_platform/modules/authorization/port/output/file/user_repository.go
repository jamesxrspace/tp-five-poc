package file

import (
	"auth_platform/core/util"
	"auth_platform/modules/authorization/domain/entity"
	"auth_platform/modules/authorization/domain/repository"
	"encoding/json"
	"errors"
	"fmt"
	"os"
)

const (
	directory = ".data"
	userFile  = ".data/user.json"
)

type UserRepository struct {
	Users map[string]*entity.User
}

var _ repository.IUserRepository = (*UserRepository)(nil)

func NewUserRepositoryWithFakeData() *UserRepository {
	repo := &UserRepository{}

	go func() {
		err := createUserFileIfNotExist()
		if err != nil {
			fmt.Println("Error initializing file:", err)
			panic(err)
		}

		data, err := loadUserDataFromFile()
		if err != nil {
			fmt.Println("Error loading JSON from file:", err)
			panic(err)
		}
		repo.Users = data
	}()

	return repo
}

func createUserFileIfNotExist() error {
	_, err := os.Stat(userFile)
	if err == nil {
		return nil
	}

	_, err = os.Stat(directory)
	if os.IsNotExist(err) {
		err := os.Mkdir(directory, 0750)
		if err != nil {
			fmt.Println("Error creating .date directory:", err)
			return err
		}
	}

	file, err := os.Create(userFile)
	if err != nil {
		fmt.Printf("Error creating file: %+v\n", err)
		return err
	}
	defer file.Close()

	users := util.GenFakeUsers(50, true)
	encoder := json.NewEncoder(file)
	err = encoder.Encode(users)
	if err != nil {
		fmt.Println("Error encoding JSON:", err)
		return err
	}

	return nil
}

func loadUserDataFromFile() (map[string]*entity.User, error) {
	file, err := os.Open(userFile)
	if err != nil {
		fmt.Println("Error opening file:", err)
		return nil, err
	}
	defer file.Close()

	var result map[string]*entity.User

	decoder := json.NewDecoder(file)
	err = decoder.Decode(&result)
	if err != nil {
		fmt.Println("Error decoding JSON:", err)
		return nil, err
	}

	return result, nil
}

func (r *UserRepository) IsReady() bool {
	return r.Users != nil
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

	file, err := os.OpenFile(userFile, os.O_RDWR, 0755)
	if err != nil {
		return err
	}
	defer file.Close()

	encoder := json.NewEncoder(file)
	err = encoder.Encode(r.Users)
	if err != nil {
		fmt.Println("Error encoding JSON:", err)
		return err
	}
	return nil
}
