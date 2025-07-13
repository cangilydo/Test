CREATE TABLE IF NOT EXISTS public."Audit"
(
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Status" integer,
    CONSTRAINT "Audit_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."Order"
(
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Status" integer,
    "CreatedOn" timestamp with time zone,
    "Type" integer,
    "UpdatedOn" timestamp with time zone,
    CONSTRAINT "Order_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."Product"
(
    "Id" uuid NOT NULL,
    "Name" character varying(200) COLLATE pg_catalog."default",
    "NormalizeName" character varying(200) COLLATE pg_catalog."default",
    "NameVector" tsvector,
    CONSTRAINT "Product_pkey" PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Product"
    OWNER to postgres;
-- Index: idx_product_name_ft

-- DROP INDEX IF EXISTS public.idx_product_name_ft;

CREATE INDEX IF NOT EXISTS idx_product_name_ft
    ON public."Product" USING gin
    ("NameVector")
    TABLESPACE pg_default;

-- Trigger: trg_set_name_eng

-- DROP TRIGGER IF EXISTS trg_set_name_eng ON public."Product";

CREATE OR REPLACE TRIGGER trg_set_name_normalize
    BEFORE INSERT
    ON public."Product"
    FOR EACH ROW
    EXECUTE FUNCTION public.set_name_normalize();

-- Trigger: trg_set_name_eng_update

-- DROP TRIGGER IF EXISTS trg_set_name_eng_update ON public."Product";

CREATE OR REPLACE TRIGGER trg_set_name_normalize_update
    BEFORE UPDATE 
    ON public."Product"
    FOR EACH ROW
    EXECUTE FUNCTION public.set_name_normalize();
	
CREATE TABLE IF NOT EXISTS public."Queue"
(
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Status" integer,
    "Type" integer,
    "RetryTime" integer,
    "ErrorMsg" text COLLATE pg_catalog."default",
    CONSTRAINT "Queue_pkey" PRIMARY KEY ("Id")
);

CREATE OR REPLACE FUNCTION public.set_create_date_order()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    NEW."CreatedOn" := now();	
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.set_name_normalize()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    NEW."NormalizeName" := unaccent(lower(NEW."Name"));
	NEW."NameVector" := to_tsvector(unaccent(lower(NEW."Name")));
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE VIEW public."V_OrderDetails"
 AS
 SELECT a."Id" AS "OrderId",
    a."ProductId",
    b."Name",
    b."NormalizeName",
    b."NameVector",
    a."Status",
    a."CreatedOn"
   FROM "Order" a
     JOIN "Product" b ON a."ProductId" = b."Id";
