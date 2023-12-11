package value_object

type ResourceOwnerIDs map[string]string

func NewResourceOwnerIDs(resourceOwnerIDs map[string]string) ResourceOwnerIDs {
	if resourceOwnerIDs == nil {
		resourceOwnerIDs = make(map[string]string)
	}
	return resourceOwnerIDs
}
