package entity

type ExampleRoom struct {
	Count int64  `bson:"count"`
	Id    string `bson:"room_id" pk:"true"`
	XrIds []string
}

func (r *ExampleRoom) Join(xrId string) {
	for _, id := range r.XrIds {
		if id == xrId {
			return
		}
	}
	r.XrIds = append(r.XrIds, xrId)
	r.Count++
}

func (r *ExampleRoom) Leave(xrId string) {
	for i, id := range r.XrIds {
		if id == xrId {
			r.XrIds = append(r.XrIds[:i], r.XrIds[i+1:]...)
			break
		}
	}
	r.Count = int64(len(r.XrIds))
}

func NewExampleRoom(id string) *ExampleRoom {
	return &ExampleRoom{
		Id:    id,
		XrIds: []string{},
	}
}
