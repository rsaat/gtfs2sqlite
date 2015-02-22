select s.* from Trip t INNER JOIN Shape s on t.shape_id = s.shape_id
where t.trip_id="1720-10-1"
ORDER BY s.shape_pt_sequence