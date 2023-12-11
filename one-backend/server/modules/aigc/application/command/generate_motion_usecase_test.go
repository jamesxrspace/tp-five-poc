package command

import (
	"context"
	"encoding/json"
	"testing"

	"gopkg.in/stretchr/testify.v1/assert"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/aigc/domain/define"
	inmem_inferencer "xrspace.io/server/modules/aigc/port/output/inferencer/inmem"
	inmem_queue "xrspace.io/server/modules/aigc/port/output/queue/inmem"
	inmem_storage "xrspace.io/server/modules/aigc/port/output/storage/inmem"
)

const (
	testInputUrl  = "testInputUrl"
	testRequestId = "test_id"
)

func TestGenerateMotionUsecase(t *testing.T) {
	type args struct {
		c *GenerateMotionCommand
	}
	tests := []struct {
		name            string
		args            args
		inferenceResult *define.InferenceResponse
		want            string
		errorCode       int
		wantErr         bool
		mustTimeout     bool
	}{
		{
			name: "success_generate_motion",
			args: args{
				c: &GenerateMotionCommand{InputUrl: testInputUrl},
			},
			inferenceResult: &define.InferenceResponse{
				OutputLocation: "s3://testBucket/path0",
				Status:         "Completed",
			},
			want:        "https://test.file.url",
			wantErr:     false,
			mustTimeout: false,
		},
		{
			name: "error_state_generate_motion",
			args: args{
				c: &GenerateMotionCommand{InputUrl: testInputUrl},
			},
			inferenceResult: &define.InferenceResponse{
				Status: "Failed",
				Error:  "error msg",
			},
			wantErr:     true,
			errorCode:   60002,
			mustTimeout: false,
		},
		{
			name: "timeout_generate_motion",
			args: args{
				c: &GenerateMotionCommand{InputUrl: testInputUrl},
			},
			wantErr:     true,
			errorCode:   60001,
			mustTimeout: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			queue := inmem_queue.NewAigcQueue()
			queue.SetMustTimeout(tt.mustTimeout)
			storage := inmem_storage.NewAigcStorage()
			inferencer := inmem_inferencer.NewAigcInferencer()
			u := NewGenerateMotionUsecase(queue, storage, inferencer)

			if tt.inferenceResult != nil {
				msg, _ := json.Marshal(tt.inferenceResult)
				_ = queue.Push(context.Background(), testRequestId, string(msg))
			}

			msgs, err := u.Execute(context.Background(), tt.args.c)
			if (err != nil) != tt.wantErr {
				t.Errorf("GenerateMotionUsecase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Equal(t, err.(*core_error.CoreError).MError.ErrorCode, tt.errorCode)
				return
			}

			assert.Equal(t, tt.want, msgs)
		})
	}
}
