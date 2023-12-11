package command

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/domain/enum"
	"xrspace.io/server/modules/feed/port/output/repository/inmem"
)

func TestDeleteReelUseCase_Execute(t *testing.T) {
	type args struct {
		cmd          *DeleteReelCommand
		defaultReels map[string]*entity.Reel
	}
	type want struct {
		reelStatus    string
		listReelTimes int
		saveReelTimes int
	}
	tests := []struct {
		args    args
		name    string
		errMsg  string
		want    want
		wantErr bool
	}{
		{
			name: "success",
			args: args{
				cmd: &DeleteReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
				defaultReels: map[string]*entity.Reel{
					"reel_id": {
						ID:     "reel_id",
						XrID:   "xrid",
						Status: enum.ReelStatusPublished,
					},
				},
			},
			want: want{
				listReelTimes: 1,
				saveReelTimes: 1,
				reelStatus:    enum.ReelStatusDeleted,
			},
		},
		{
			name: "failed_if_reel_not_found",
			args: args{
				cmd: &DeleteReelCommand{
					ReelID: "reel_id",
					XrID:   "xrid",
				},
			},
			wantErr: true,
			want: want{
				listReelTimes: 1,
			},
			errMsg: "reel not found",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			reelRepo := newMockDeleteReelRepository(tt.args.defaultReels)
			queryRepo := newMockQueryReelRepository(tt.args.defaultReels)
			dep := define.Dependency{ReelRepo: reelRepo, QueryRepo: queryRepo}
			u := NewDeleteReelUseCase(dep)

			// act
			_, err := u.Execute(ctx, tt.args.cmd)
			if (err != nil) != tt.wantErr {
				t.Errorf("DeleteReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			// assert
			assert.Equal(t, tt.want.listReelTimes, queryRepo.listReelTimes)
			assert.Equal(t, tt.want.saveReelTimes, reelRepo.saveReelTimes)

			got, err := queryRepo.ListReel(ctx, &define.ListReelFilter{
				ReelID: tt.args.cmd.ReelID,
			})
			assert.NoError(t, err)
			if len(got.Items) > 0 {
				assert.Equal(t, tt.want.reelStatus, got.Items[0].Status)
			}
		})
	}
}

type mockDeleteReelRepository struct {
	*inmem.ReelRepository
	saveReelTimes int
}

func newMockDeleteReelRepository(defaultReels map[string]*entity.Reel) *mockDeleteReelRepository {
	return &mockDeleteReelRepository{
		ReelRepository: inmem.NewReelRepository(defaultReels),
	}
}

func (m *mockDeleteReelRepository) Save(ctx context.Context, reel *entity.Reel) error {
	m.saveReelTimes++
	return m.ReelRepository.Save(ctx, reel)
}

type mockQueryReelRepository struct {
	*inmem.QueryRepository
	listReelTimes int
}

func newMockQueryReelRepository(defaultReels map[string]*entity.Reel) *mockQueryReelRepository {
	return &mockQueryReelRepository{
		QueryRepository: inmem.NewQueryRepository(nil, nil, nil, defaultReels),
	}
}

func (m *mockQueryReelRepository) ListReel(ctx context.Context, filter *define.ListReelFilter) (*define.ListReelResult, error) {
	m.listReelTimes++
	return m.QueryRepository.ListReel(ctx, filter)
}
