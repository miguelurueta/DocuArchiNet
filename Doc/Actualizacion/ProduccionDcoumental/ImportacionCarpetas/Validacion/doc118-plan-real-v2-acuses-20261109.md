# DOC-118 — estado del plan tras preparar pérdida de acuses

Complementa los planes RV-08 y de enlace/driver conservando sus casos, recursos, topes y cortes. No concede autorización de ejecución o despliegue.

| Dependencia | Estado |
| --- | --- |
| P1/P2 | Implementados y probados en candidato local, entrega6.9/6.10 conservada. |
| P3 | Paquete de decorador/conductor implementado y probado localmente en6.11. Excluido del producto; no instalado. El host debe enlazar callback nativo, proyección SELECT autorizada y cierre del control en el ambiente de prueba. |
| P4 | Sitio, proveedor físico, MySQL/IIS, observador y enlace/lector del host sin calificar. Requiere identificar ambiente/cuentas y alcance de despliegue/calificación; pruebas sintéticas no lo acreditan. |
| P5 | Ejecución real sin autorizar. Fijar SHA de candidato/paquete/runner y recursos concretos para revisión final; no reutilizar sesiones agotadas o recursos históricos. |

El conductor Q1/Q2/Q3 limita cada caso a1 ALMACENAR/2 recuperaciones/4 lecturas; Q2 usa1/1/3. No equivale al presupuesto global ni cuenta por sí solo SELECT físicos o navegador; el host conserva beforeDispatch y el presupuesto existente. Q4 usa el observador existente, sujeto a P4. Timeout de driver/lector/arm es responsabilidad del host; la cota temporal del conductor se comprueba entre operaciones. No solicitar ejecución hasta tener un enlace calificado y una propuesta revisable de ambiente, cuentas, recursos y topes. Sin nuevas aperturas, SQL o HTTP reales aquí.

Se conserva la propuesta original de2 lotes/4 raíces/4 PDFs,2 sesiones,4 expedientes,2 invocaciones documentales máximas,1000 HTTP totales/100 de negocio y180 SELECT; no son cupo autorizado. Históricos18/21/24 y41/53/69 excluidos. Consumo real histórico1276/113/9, incremento0/0/0. COORD-02/03 necesitan evidencia independiente. Resultado: plan NO_EJECUTABLE/NO_AUTORIZADO, P3 local preparado y P4/P5 abiertos. Detalle y resultados en [reporte local](doc118-acuses-v2-local-20261109.md).
