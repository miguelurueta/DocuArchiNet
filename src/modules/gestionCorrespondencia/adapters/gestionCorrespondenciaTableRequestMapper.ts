import {
  mapDynamicUiServerTableRequest,
  type DynamicUiServerTableRequest,
} from "../../../app/Components/UI/AppTable/utils/dynamicUiTableRequestMapper";
import type { DynamicTableQueryInput } from "../../../app/Components/UI/AppTable/types/dynamicUiTableQuery.types";

export type GestionCorrespondenciaTableRequest = DynamicUiServerTableRequest;

const resolveGestionCorrespondenciaSearchType = (
  input: DynamicTableQueryInput,
): number | undefined => {
  if (input.searchType === 3) {
    return 3;
  }

  if (input.search?.trim()) {
    return 2;
  }

  return input.searchType;
};

export const mapGestionCorrespondenciaTableRequest = (
  input: DynamicTableQueryInput,
): GestionCorrespondenciaTableRequest => ({
  ...mapDynamicUiServerTableRequest(input),
  SearchType: resolveGestionCorrespondenciaSearchType(input),
});
