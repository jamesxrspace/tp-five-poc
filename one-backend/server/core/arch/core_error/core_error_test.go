package core_error

import "testing"

func TestModuleError_Equals(t *testing.T) {
	type fields struct {
		ModuleName string
		ModuleCode int
		ErrorCode  int
	}
	type args struct {
		m2 ModuleError
	}
	tests := []struct {
		name   string
		fields fields
		args   args
		want   bool
	}{
		{
			name: "module error should equals",
			fields: fields{
				ModuleName: "m1",
				ModuleCode: 0,
				ErrorCode:  0,
			},
			args: args{
				m2: ModuleError{
					ModuleName: "m1",
					ModuleCode: 0,
					ErrorCode:  0,
				},
			},
			want: true,
		},
		{
			name: "module error should not equals",
			fields: fields{
				ModuleName: "m1",
				ModuleCode: 1,
				ErrorCode:  0,
			},
			args: args{
				m2: ModuleError{
					ModuleName: "m1",
					ModuleCode: 0,
					ErrorCode:  0,
				},
			},
			want: false,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			m := &ModuleError{
				ModuleName: tt.fields.ModuleName,
				ModuleCode: tt.fields.ModuleCode,
				ErrorCode:  tt.fields.ErrorCode,
			}
			if got := m.Equals(tt.args.m2); got != tt.want {
				t.Errorf("Equals() = %v, want %v", got, tt.want)
			}
		})
	}
}
