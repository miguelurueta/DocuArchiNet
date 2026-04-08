export type WorkflowInboxAutocompleteRequest = {
  search: string;
  limit: number;
};

export type WorkflowInboxAutocompleteBackendItem = {
  value?: string | null;
  Value?: string | null;
  label?: string | null;
  Label?: string | null;
  field?: string | null;
  Field?: string | null;
};

export type WorkflowInboxAutocompleteBackendResponse = {
  items?: WorkflowInboxAutocompleteBackendItem[] | null;
  Items?: WorkflowInboxAutocompleteBackendItem[] | null;
};

export type WorkflowInboxAutocompleteItem = {
  value: string;
  label?: string;
};
