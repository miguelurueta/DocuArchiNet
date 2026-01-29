/**
 * Representa un ítem crudo del menú que llega desde la API.
 */
export interface RawMenuItem {
  IdMenuPrincipal: number;
  NombreModulo: string;
  ValueNode: string;
  ToltipNode: string;
  UrlNode: string;
  PageName: string;
  VisibleNode: number;
  NodoPlantillaRadicado: string;
  TipoPlantilla: string;
  IdPlantilla: number;
  UrlExterna: string;
  UrlContent: string;
  ValueContent: string;
  ValueCard: string;
  ValueCardConten: string;
  TIpoModulo: string;
  AcesoDirecto: number;
  IdPadre: number;
  Orden: number;
  Icono: string;
}

/**
 * Nodo jerárquico del menú para renderizar en el sidebar.
 */
export interface MenuNode extends RawMenuItem {
  children: MenuNode[];
}
