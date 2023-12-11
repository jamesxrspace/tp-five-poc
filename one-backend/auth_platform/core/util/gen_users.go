package util

import (
	"auth_platform/modules/authorization/domain/entity"
	"fmt"

	"github.com/google/uuid"
	"golang.org/x/crypto/bcrypt"
)

func GenFakeUsers(num int, useUuid bool) map[string]*entity.User {
	users := make(map[string]*entity.User)

	for i := 1000; i < 1000+num; i++ {
		userID := fmt.Sprintf("xrspacetest%d", i)
		if useUuid {
			userID = uuid.NewString()
		}

		users[userID] = entity.NewUser(
			userID,
			fmt.Sprintf("xrspacetest%d", i),
			fmt.Sprintf("xrspacetest%d_nickname", i),
			"1qazXSW@",
			fmt.Sprintf("xrspacetest%d@xrspace.io", i),
			"",
			i%2 == 0,
			bcrypt.MinCost,
		)
	}

	return users
}
