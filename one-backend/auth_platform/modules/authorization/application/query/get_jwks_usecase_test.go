package query

import (
	"context"
	"testing"

	"encoding/json"

	service "auth_platform/core/service/jwt"

	"github.com/stretchr/testify/suite"
)

type TestGetJwksUsecaseTestSuite struct {
	suite.Suite
}

func TestGetJwks(t *testing.T) {
	suite.Run(t, new(TestGetJwksUsecaseTestSuite))
}

func (s *TestGetJwksUsecaseTestSuite) TestGetJwksUsecase_Execute() {
	type args struct {
		ctx context.Context
	}

	type want struct {
		responseJSONStr string
	}

	jwks := map[string][]map[string]string{
		"keys": {
			map[string]string{
				"kty": "RSA",
				"e":   "AQAB",
				"n":   "4s75KZJXXgNj4Rz-hYWiN69C-fHaAemR_9tLTCkqO3ONDk61jJlakSi5MfFgBiRxtFVv2ro-e_A5EW8r5NWz-h25YW9ZCc-KDWSxM481FnSEIuIlpTRTgZex_XesdLfonU-GiqIbbUGFoJKZZM0K-lLyXLLKTjy2dOnUy730Z-ctins1haATIrEkbRk1t7nbyz8Bge6KBFbwwr8tMjYkOR2o6qz439F9vK78FcqzCVb_bpVh12bD9jEp0BM40Ko_CXXIyoHJrgVjdsSwkixPZGIQaUMXDPGLqL431DyZ2WzZ4Lv5kQDQHtdgpjZinZi3aNvGxt1-Z2iumSi5WvBZMw",
				"alg": "RS256",
				"use": "sig",
				"kid": "XR",
			},
		},
	}

	jwksJSONStr, _ := json.Marshal(jwks)

	tests := []struct {
		args       args
		name       string
		want       want
		wantErrMsg string
		wantErr    bool
	}{
		{
			name: "success",
			args: args{
				ctx: context.Background(),
			},
			want: want{
				responseJSONStr: string(jwksJSONStr),
			},
		},
	}

	for _, tt := range tests {
		s.Run(tt.name, func() {
			// arrange
			jwtService := service.NewJwtService(privateKeyPath, mockAppID, mockPooID, mockSecret)
			uc := NewGetJwksUsecase(jwtService)

			// act
			got := uc.Execute(tt.args.ctx)
			buf, _ := json.Marshal(got)

			// assert
			s.Equal(tt.want.responseJSONStr, string(buf))
		})
	}
}
