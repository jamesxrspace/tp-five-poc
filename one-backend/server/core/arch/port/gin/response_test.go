package gin

import (
	"encoding/json"
	"errors"
	"fmt"
	"net/http"
	"net/http/httptest"
	"testing"

	"github.com/gin-gonic/gin"
	"gopkg.in/stretchr/testify.v1/assert"

	"xrspace.io/server/core/arch/core_error"
)

// nolint
func Test_response(t *testing.T) {
	var GetAccountError = core_error.ModuleError{ModuleName: "account", ErrorCode: 10002}
	type args struct {
		givenError         error
		expectHttpCode     int
		expectResponseBody *ResponseBody
	}
	tests := []struct {
		name string
		args args
	}{
		{
			name: "WhenGiveNoError_ShouldReturn200",
			args: args{
				givenError:         nil,
				expectHttpCode:     http.StatusOK,
				expectResponseBody: &ResponseBody{},
			},
		},
		{
			name: "WhenGiveCoreError_ShouldReturnCorrectMessage",
			args: args{
				givenError: core_error.NewCoreError(
					GetAccountError,
					fmt.Errorf("test error"),
				),
				expectHttpCode: http.StatusBadRequest,
				expectResponseBody: &ResponseBody{
					ErrorCode: GetAccountError.ErrorCode,
					Message:   "test error",
				},
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			w := httptest.NewRecorder()
			ctx, _ := gin.CreateTestContext(w)
			ctx.Request = httptest.NewRequest("GET", "/", nil)

			// Act
			response(ctx, nil, tt.args.givenError)

			// Assert
			actual := &ResponseBody{}
			_ = json.Unmarshal(w.Body.Bytes(), actual)

			assert.Equal(t, tt.args.expectResponseBody, actual)
			assert.Equal(t, tt.args.expectHttpCode, w.Code)
		})
	}
}

func Test_responseV2(t *testing.T) {

	type args struct {
		givenError         error
		expectHttpCode     int
		expectResponseBody *ResponseBodyV2
	}
	tests := []struct {
		name string
		args args
	}{
		{
			name: "WhenGiveCodeErrors_ShouldReturnBadRequestAndCorrectErrorCode",
			args: args{
				givenError:     core_error.NewCodeError("0001", errors.New("test error")),
				expectHttpCode: http.StatusBadRequest,
				expectResponseBody: &ResponseBodyV2{
					ErrorCode: "0001",
					Message:   "test error",
				},
			},
		},
		{
			name: "WhenGiveOtherError_ShouldReturnInternalServerError",
			args: args{
				givenError:     fmt.Errorf("test internal error"),
				expectHttpCode: http.StatusInternalServerError,
				expectResponseBody: &ResponseBodyV2{
					Message: "test internal error",
				},
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			w := httptest.NewRecorder()
			ctx, _ := gin.CreateTestContext(w)
			ctx.Request = httptest.NewRequest(http.MethodGet, "/", nil)

			// Act
			response(ctx, nil, tt.args.givenError)

			// Assert
			actual := &ResponseBodyV2{}
			_ = json.Unmarshal(w.Body.Bytes(), actual)

			assert.Equal(t, tt.args.expectResponseBody, actual)
			assert.Equal(t, tt.args.expectHttpCode, w.Code)
		})
	}
}
