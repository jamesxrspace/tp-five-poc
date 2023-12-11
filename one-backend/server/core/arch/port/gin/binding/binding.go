package binding

import (
	"io"

	"github.com/gin-gonic/gin"
	"github.com/gin-gonic/gin/binding"
	"github.com/go-errors/errors"
	"github.com/hashicorp/go-multierror"
)

func ShouldBind(ctx *gin.Context, obj any) error {
	// ignore validator when binding
	tmpValidator := binding.Validator
	binding.Validator = nil
	defer func() {
		binding.Validator = tmpValidator
	}()

	// ignore error when binding
	err := multierror.Append(nil, ctx.ShouldBindUri(obj)) // use 'uri:"param"' in binding tag to bind http://domain/:param
	err = multierror.Append(err, ctx.ShouldBindQuery(obj))
	err = multierror.Append(err, ctx.ShouldBindHeader(obj))

	// bind body FORM / JSON
	var errBindBody error
	switch ctx.ContentType() {
	case binding.MIMEJSON:
		errBindBody = ctx.ShouldBind(&obj)
	default:
		errBindBody = ctx.ShouldBind(obj)
	}

	// we accept empty body of request, so we have to ignore the EOF error when binding form or json
	if !errors.Is(errBindBody, io.EOF) {
		err = multierror.Append(err, errBindBody)
	}

	// bind xrid, app_id, access_token, resource_owner_id, issuer, azp
	keys := []string{"xrid", "app_id", "access_token", "resource_owner_id", "issuer", "azp"}
	m := make(map[string][]string)
	for _, key := range keys {
		v, isExist := ctx.Get(key)
		if isExist {
			m[key] = []string{v.(string)}
		}
	}

	// ignore error when binding
	_ = binding.MapFormWithTag(obj, m, "token")

	return err.ErrorOrNil()
}
