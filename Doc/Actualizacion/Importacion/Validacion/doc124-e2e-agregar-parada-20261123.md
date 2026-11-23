# DOC124: parada del preflight de AGREGAR

La ventana e3a3f1afc41c46ba85dfffabb537e3e1 quedó STOPPED y cerrada el2026-11-23
a15:47:08 UTC, después de confirmación específica en TTY. El control del propietario
no acreditó conjuntamente usuario141/empresa2/permiso: DOC124_OWNER_PERMISSION_CONTEXT.
Solo se ejecutó un SELECT; cero login web, solicitudes de servicios Excel, archivos
recibidos, cargas o cambios de configuración. Las claves se eliminaron. No hay
PASS real de AGREGAR ni se aplicó SQL17.

Ese código agrupaba varios motivos y no permite reconstruir cuál de sus condiciones
falló; no se registró ni recuperó la cuenta escrita por el operador. La investigación
posterior, con recursos del módulo, confirmó por dos SELECT que141 sigue activo,
empresa2 y ADMINISTRAR_CARGAS_IMPORTACION=1. El verificador existente DOC115 confirmó
que el DSN MembershipUsers corresponde al destino autorizado: solo leyó SERVER y
DATABASE registrados, sin UID/PWD. No se cambió DSN, permiso o conexión.

Se preparó la continuación589a3040b87d2ae256e7f748296ec090, sin reabrir la anterior.
La cuenta se resuelve por el ID141 autorizado y su login permanece únicamente en
memoria; la consola solicita su contraseña, sin volver a teclear el nombre.
Los diagnósticos ahora distinguen cuenta activa/permiso, empresa e identidad.
Se reutilizan los tres archivos íntegros y el mismo arnés, sin regenerar claves ni
modificar los originales. Aún requiere confirmación TTY y claves nuevas.

Consumo conservado:1 SELECT de la ventana detenida más2 SELECT de diagnóstico;
la continuación admite77 SELECT más, total80. Cero de60 llamadas de servicios
consumidas. La admisión nueva se reduce a18min; el intervalo entre confirmación y
cierre anterior fue menor de67s, manteniendo el trabajo admitido conjunto bajo20min.
La espera de captura de claves se distingue del tiempo de admisión de servicios.
Los efectos máximos siguen siendo tres cargas/archivos, sin importación, DDL,
plantillas, permisos, limpieza ni SQL17. Las ventanas y sus claves no se reutilizan.

Este informe acredita la parada y la preparación de la continuación, no su ejecución.
Estado y plan privados en `.tmp/doc124-real/<ventana>/`; no incluyen credenciales.
La matriz de AGREGAR/REEMPLAZAR y las tareas2.6/3.2 siguen pendientes.

Relanzamiento posterior solicitado por el usuario «lanza de nuevo»:589a3040 quedó
interrumpida durante la captura de claves, con0 SELECT/servicios y sin completar
su finally. Se comprobaron ausentes el proceso de consola39752 y su hijo Node;
se conservó el estado original y se añadió un registro de interrupción separado.
No se recuperaron secretos ni se afirmó una limpieza de finally no registrada.
Nueva ventana d28f908a93171ecf8f8d413d90293604, consola41128, mismo alcance y
archivos íntegros,77 SELECT adicionales/60 servicios/18min y3 SELECT previos
conservados. La instrucción expresa de relanzamiento autoriza esta nueva ventana;
se capturan claves nuevas sin solicitar de nuevo el SI del mismo alcance.
Su apertura no acredita login, ejecución o PASS de AGREGAR.
