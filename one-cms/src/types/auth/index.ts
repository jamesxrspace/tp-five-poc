export interface Permission {
  id: string;
  name: string;
  type: 'read' | 'write';
}

export interface Role {
  id: string;
  name: string;
  permissions: Permission[];
}
