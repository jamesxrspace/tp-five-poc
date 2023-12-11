package error_core

var _ IError = (*ErrorStruct)(nil)

type IError interface {
	Error() string
	GetErrCode() int
	GetData() interface{}
}

type ErrorStruct struct {
	Data    interface{}
	Err     error
	ErrCode int
}

func NewErrorStruct(errCode int, err error, data interface{}) *ErrorStruct {
	return &ErrorStruct{
		Err:     err,
		ErrCode: errCode,
		Data:    data,
	}
}

func (e *ErrorStruct) Error() string {
	return e.Err.Error()
}

func (e *ErrorStruct) GetErrCode() int {
	return e.ErrCode
}

func (e *ErrorStruct) GetData() interface{} {
	return e.Data
}
