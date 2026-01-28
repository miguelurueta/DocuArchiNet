import { useListadoEntidad } from "../../../api/reactQuery/useListadoEntidad";
import type ListaEmpresaDTO from "../models/ListaEmpresaDTO";

export function useEmpresaActual() {
  const query = useListadoEntidad<ListaEmpresaDTO>(
    "/api/accout/SolicitaEstructuraEmpresa",
    "empresa-actual",
    {
      page: 1,
      pageSize: 1,
    }
  );
  
  return {
    empresa: query.data?.data?.[0] ?? null,
    empresas: query.data?.data ?? [],
    isLoading: query.isLoading,
    isError: query.isError,
    error: query.error,
  };
}
