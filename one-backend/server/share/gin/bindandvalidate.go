package gin

import (
	"github.com/gin-gonic/gin"

	"xrspace.io/server/core/infra/validator"
)

func BindAndValidate(c *gin.Context, v any) error {
	if err := c.ShouldBind(v); err != nil {
		return err
	}

	if j, ok := v.(validator.IValidator); ok {
		return j.Validate()
	}
	return nil
}
