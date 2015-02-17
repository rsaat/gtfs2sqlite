SELECT st.trip_id, st.stop_sequence,st.departure_time,st.shape_dist_traveled,s.stop_name,s.stop_lat,s.stop_lon from  
StopTime st 
INNER JOIN Trip t ON t.trip_id = st.trip_id 
INNER JOIN Stop s ON s.stop_id = st.stop_id 
WHERE st.trip_id = "1720-10-1"
ORDER BY st.trip_id, st.stop_sequence 