-- WARNING: This schema is for context only and is not meant to be run.
-- Table order and constraints may not be valid for execution.

CREATE TABLE public.areas (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  name text NOT NULL UNIQUE,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT areas_pkey PRIMARY KEY (id)
);
CREATE TABLE public.categories (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  name text NOT NULL UNIQUE,
  thumbnail_url text,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT categories_pkey PRIMARY KEY (id)
);
CREATE TABLE public.favorites (
  user_id uuid NOT NULL,
  recipe_id text NOT NULL,
  CONSTRAINT favorites_pkey PRIMARY KEY (user_id, recipe_id),
  CONSTRAINT favorites_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id)
);
CREATE TABLE public.ingredients (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  name text NOT NULL UNIQUE,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT ingredients_pkey PRIMARY KEY (id)
);
CREATE TABLE public.recipe_areas (
  recipe_id text NOT NULL,
  area_id uuid NOT NULL,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT recipe_areas_pkey PRIMARY KEY (recipe_id, area_id),
  CONSTRAINT fk_recipe_areas_recipe FOREIGN KEY (recipe_id) REFERENCES public.recipes(id),
  CONSTRAINT fk_recipe_areas_area FOREIGN KEY (area_id) REFERENCES public.areas(id)
);
CREATE TABLE public.recipe_categories (
  recipe_id text NOT NULL,
  category_id uuid NOT NULL,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT recipe_categories_pkey PRIMARY KEY (recipe_id, category_id),
  CONSTRAINT fk_recipe_categories_recipe FOREIGN KEY (recipe_id) REFERENCES public.recipes(id),
  CONSTRAINT fk_recipe_categories_category FOREIGN KEY (category_id) REFERENCES public.categories(id)
);
CREATE TABLE public.recipe_ingredients (
  recipe_id text NOT NULL,
  ingredient_id uuid NOT NULL,
  quantity text,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT recipe_ingredients_pkey PRIMARY KEY (recipe_id, ingredient_id),
  CONSTRAINT fk_recipe_ingredients_recipe FOREIGN KEY (recipe_id) REFERENCES public.recipes(id),
  CONSTRAINT fk_recipe_ingredients_ingredient FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(id)
);
CREATE TABLE public.recipe_steps (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  recipe_id text NOT NULL,
  step_number integer NOT NULL,
  instruction text NOT NULL,
  created_at timestamp with time zone DEFAULT now(),
  CONSTRAINT recipe_steps_pkey PRIMARY KEY (id),
  CONSTRAINT fk_recipe_steps_recipe FOREIGN KEY (recipe_id) REFERENCES public.recipes(id)
);
CREATE TABLE public.recipes (
  id text NOT NULL,
  str_meal text NOT NULL,
  str_category text,
  str_area text,
  str_instructions text,
  str_meal_thumb text,
  str_tags text,
  str_youtube text,
  str_source text,
  str_image_source text,
  str_creative_commons_confirmed text,
  date_modified text,
  created_at timestamp with time zone DEFAULT now(),
  updated_at timestamp with time zone DEFAULT now(),
  str_ingredients text,
  CONSTRAINT recipes_pkey PRIMARY KEY (id)
);
CREATE TABLE public.users (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  first_name text NOT NULL,
  last_name text NOT NULL,
  email text NOT NULL UNIQUE,
  role text NOT NULL DEFAULT 'user'::text CHECK (role = ANY (ARRAY['user'::text, 'admin'::text])),
  CONSTRAINT users_pkey PRIMARY KEY (id)
);

-- Enable RLS for all tables
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.favorites ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.recipes ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.categories ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.recipe_categories ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.recipe_areas ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.recipe_ingredients ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.recipe_steps ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.ingredients ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.areas ENABLE ROW LEVEL SECURITY;

-- USERS
CREATE POLICY "users_select_self_or_admin" ON public.users
  FOR SELECT USING (id = auth.uid() OR role = 'admin');
CREATE POLICY "users_manage_admin" ON public.users
  FOR ALL USING (role = 'admin');

CREATE POLICY "users_insert_self"
  ON public.users
  FOR INSERT
  WITH CHECK (true);

-- FAVORITES
CREATE POLICY "favorites_manage_own_or_admin" ON public.favorites
  FOR ALL USING (user_id = auth.uid() OR EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- RECIPES
CREATE POLICY "recipes_select_all" ON public.recipes
  FOR SELECT USING (true);
CREATE POLICY "recipes_manage_admin" ON public.recipes
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- CATEGORIES
CREATE POLICY "categories_select_all" ON public.categories
  FOR SELECT USING (true);
CREATE POLICY "categories_manage_admin" ON public.categories
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- RECIPE_CATEGORIES
CREATE POLICY "recipe_categories_select_all" ON public.recipe_categories
  FOR SELECT USING (true);
CREATE POLICY "recipe_categories_manage_admin" ON public.recipe_categories
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- RECIPE_AREAS
CREATE POLICY "recipe_areas_select_all" ON public.recipe_areas
  FOR SELECT USING (true);
CREATE POLICY "recipe_areas_manage_admin" ON public.recipe_areas
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- RECIPE_INGREDIENTS
CREATE POLICY "recipe_ingredients_select_all" ON public.recipe_ingredients
  FOR SELECT USING (true);
CREATE POLICY "recipe_ingredients_manage_admin" ON public.recipe_ingredients
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- RECIPE_STEPS
CREATE POLICY "recipe_steps_select_all" ON public.recipe_steps
  FOR SELECT USING (true);
CREATE POLICY "recipe_steps_manage_admin" ON public.recipe_steps
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- INGREDIENTS
CREATE POLICY "ingredients_select_all" ON public.ingredients
  FOR SELECT USING (true);
CREATE POLICY "ingredients_manage_admin" ON public.ingredients
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));

-- AREAS
CREATE POLICY "areas_select_all" ON public.areas
  FOR SELECT USING (true);
CREATE POLICY "areas_manage_admin" ON public.areas
  FOR ALL USING (EXISTS(SELECT 1 FROM public.users u WHERE u.id = auth.uid() AND u.role='admin'));
