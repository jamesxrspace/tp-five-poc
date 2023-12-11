package responser

import (
	"auth_platform/core/error_core"
	auth_errors "auth_platform/modules/authorization/domain/errors"
	"fmt"
	"net/http"

	"github.com/gin-gonic/gin"
)

type IHttpResponser interface{}

type HttpResponse struct {
	Data    any    `json:"data,omitempty"`
	Msg     string `json:"msg,omitempty"`
	ErrCode int    `json:"err_code,omitempty"`
}

type HttpResponser struct {
	c *gin.Context
}

func NewHttpResponser(c *gin.Context) *HttpResponser {
	return &HttpResponser{
		c: c,
	}
}

func (r *HttpResponser) Response(data any, err error) {
	if err != nil {
		r.fail(err)
		return
	}
	r.success(data)
}

func (r *HttpResponser) ResponseWithHtml(page, title string) {
	r.c.HTML(http.StatusOK, page+".html", gin.H{
		"title": title,
	})
}

func (r *HttpResponser) success(data any) {
	r.c.JSON(http.StatusOK, data)
}

func (r *HttpResponser) fail(err error) {
	if newErr, ok := err.(error_core.IError); ok {

		switch newErr.GetErrCode() {
		case
			auth_errors.DecodeClaimsFailed,
			auth_errors.FindProfileFailed,
			auth_errors.GenRandFailed,
			auth_errors.GenRefreshTokenFailed,
			auth_errors.GenTokenFailed,
			auth_errors.GetAccountInfoFailed,
			auth_errors.InvalidArguments,
			auth_errors.InvalidEmailOrPassword,
			auth_errors.InvalidSignMethod,
			auth_errors.InvalidToken,
			auth_errors.JsonMarshalFailed,
			auth_errors.ParsePrivateKeyFailed,
			auth_errors.ParsePublicKeyFailed,
			auth_errors.ParseTokenFailed,
			auth_errors.ReadPrivateKeyFailed,
			auth_errors.ReadPublicKeyFailed,
			auth_errors.SignTokenFailed,
			auth_errors.TokenExpired,
			auth_errors.UnableGetJwks:
			r.badRequest(newErr)
			return
		default:
			r.internalServerError(newErr)
			return
		}

	}
	r.c.JSON(http.StatusInternalServerError, &HttpResponse{
		Msg:     err.Error(),
		ErrCode: 90000,
	})
}

func (r *HttpResponser) badRequest(err error_core.IError) {
	r.c.JSON(http.StatusBadRequest, &HttpResponse{
		Msg:     err.Error(),
		ErrCode: err.GetErrCode(),
	})
}

func (r *HttpResponser) internalServerError(err error_core.IError) {
	fmt.Println(err.Error(), err.GetErrCode())
	r.c.JSON(http.StatusInternalServerError, &HttpResponse{
		Msg:     err.Error(),
		ErrCode: err.GetErrCode(),
	})
}
