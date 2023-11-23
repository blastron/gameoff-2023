=== test_walkaround_outside ===
~ walkaround_mode("test_outdoors", "spawn")
+   [Pet the cat. #pet_cat]
    ~ dialogue_mode()
    You pet the cat. What a good cat.
    -> test_walkaround_outside
+   [Enter the door. #enter_shop]
    -> test_walkaround_inside

=== test_walkaround_inside ===
~ walkaround_mode("test_indoors", "spawn")
*   [Talk to the shopkeeper. #talk_shopkeeper]
    ~ dialogue_mode()
    The shopkeeper doesn't seem to have anything to say.
    -> test_walkaround_inside
+   [Exit the shop. #exit_shop]
    ~ walkaround_mode("test_outdoors", "shop_door")
    -> test_walkaround_outside