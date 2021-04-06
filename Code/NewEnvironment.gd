extends WorldEnvironment

func _on_Sky_texture_sky_updated():
	get_parent().get_node("Sky_texture").copy_to_environment(environment)
