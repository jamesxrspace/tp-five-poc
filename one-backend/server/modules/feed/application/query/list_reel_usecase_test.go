package query

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"

	"xrspace.io/server/core/arch/port/pagination"
	"xrspace.io/server/modules/feed/application/define"
	"xrspace.io/server/modules/feed/domain/entity"
	"xrspace.io/server/modules/feed/port/output/repository/inmem"
)

func TestListReelUseCase_Execute(t *testing.T) {
	type args struct {
		query        *ListReelQuery
		defaultReels map[string]*entity.Reel
	}
	type want struct {
		listReelTimes int
		reelCount     int
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
				query: &ListReelQuery{
					PaginationQuery: pagination.PaginationQuery{
						Size: 2,
					},
				},
				defaultReels: map[string]*entity.Reel{
					"reel1": {
						ID: "reel1",
					},
					"reel2": {
						ID: "reel2",
					},
				},
			},
			want: want{
				listReelTimes: 1,
				reelCount:     2,
			},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// arrange
			ctx := context.Background()
			repo := newMockListReelRepository(tt.args.defaultReels)
			dep := define.Dependency{QueryRepo: repo}
			u := NewListReelUseCase(dep)

			// act
			got, err := u.Execute(ctx, tt.args.query)
			if (err != nil) != tt.wantErr {
				t.Errorf("ListReelUseCase.Execute() error = %v, wantErr %v", err, tt.wantErr)
				return
			}

			if tt.wantErr {
				assert.Contains(t, err.Error(), tt.errMsg)
				return
			}

			result := got.(*ListReelResponse)

			// assert
			assert.Equal(t, tt.want.listReelTimes, repo.listReelTimes)
			assert.Equal(t, tt.want.reelCount, len(result.Items))
		})
	}
}

type mockListReelRepository struct {
	*inmem.QueryRepository
	listReelTimes int
}

func newMockListReelRepository(defaultReels map[string]*entity.Reel) *mockListReelRepository {
	return &mockListReelRepository{
		QueryRepository: inmem.NewQueryRepository(nil, nil, nil, defaultReels),
	}
}

func (m *mockListReelRepository) ListReel(ctx context.Context, filter *define.ListReelFilter) (*define.ListReelResult, error) {
	m.listReelTimes++
	return m.QueryRepository.ListReel(ctx, filter)
}
