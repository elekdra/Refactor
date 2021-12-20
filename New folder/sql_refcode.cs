public void UpdateSampleSets(SampleSetModel[] sampleSets)
{
    DataAccess dao = new DataAccess();
    SQLiteConnection connection;
    SQLiteCommand cmd;
    const BLANK=1;
    const SAMPLE=2;


    
    for (int i = sampleSets.Length - 1; i >= 0; i--)
    {
        connection = dao.GetConnectionObject();
        cmd = new SQLiteCommand(connection);
        cmd.CommandText = "SELECT createdFromcal FROM tb_sample_sets WHERE sampleTableID =@sampleTableId";
        cmd.Parameters.AddWithValue("@sampleTableId", sampleSets[i].SampleTableId);
        var list1 = dao.ExecuteQueryParameterised(cmd, connection);

        bool isCalTable = (list1[0]["createdFromcal"] == "True") ? true : false;

        connection = dao.GetConnectionObject();
        cmd = new SQLiteCommand(connection);

        if (sampleSets[i].SampleTypeId == BLANK && isCalTable) // 
        {
            cmd.CommandText = "UPDATE tb_sample_sets SET sampleTableId =@sampleTableId, user =@user, sampleName =@sampleName, asPos =@asPos, weight =@weight, sampleTypeId =@sampleTypeId, methodId =@methodId,"
            + " calibrationId =@calibrationId, protein =@protein, moisture =@moisture, comment1 =@comment1, comment2 =@comment2, asPosRank =@asPosRank WHERE sampleID =@sampleId";
            cmd.Parameters.AddWithValue("@calibrationId", sampleSets[i].CalibrationId);
        }
        else if (sampleSets[i].SampleTypeId == BLANK)
        {
            cmd.CommandText = "UPDATE tb_sample_sets SET sampleTableId =@sampleTableId, user =@user, sampleName =@sampleName, asPos =@asPos, weight =@weight, sampleTypeId =@sampleTypeId, methodId =@methodId,"
            + " standardId =@standardId, calibrationId =@calibrationId, protein =@protein, moisture =@moisture, comment1 =@comment1, comment2 =@comment2, asPosRank =@asPosRank WHERE sampleID =@sampleId";
            cmd.Parameters.AddWithValue("@standardId", null);
            cmd.Parameters.AddWithValue("@calibrationId", null);
        }
        else if (sampleSets[i].SampleTypeId == SAMPLE)
        {
            cmd.CommandText = "UPDATE tb_sample_sets SET sampleTableId =@sampleTableId, user =@user, sampleName =@sampleName, asPos =@asPos, weight =@weight, sampleTypeId =@sampleTypeId, methodId =@methodId,"
            + " standardId =@standardId, calibrationId =@calibrationId, protein =@protein, moisture =@moisture, comment1 =@comment1, comment2 =@comment2, asPosRank =@asPosRank WHERE sampleID =@sampleId";
            cmd.Parameters.AddWithValue("@standardId", null);
            cmd.Parameters.AddWithValue("@calibrationId", sampleSets[i].CalibrationId);
        }
        else
        {
            cmd.CommandText = "UPDATE tb_sample_sets SET sampleTableId =@sampleTableId, user =@user, sampleName =@sampleName, asPos =@asPos, weight =@weight, sampleTypeId =@sampleTypeId, methodId =@methodId,"
            + " standardId =@standardId, calibrationId =@calibrationId, protein =@protein, moisture =@moisture, comment1 =@comment1, comment2 =@comment2, asPosRank =@asPosRank WHERE sampleID =@sampleId";
            cmd.Parameters.AddWithValue("@standardId", sampleSets[i].StandardId);
            cmd.Parameters.AddWithValue("@calibrationId", sampleSets[i].CalibrationId);
        }

        cmd.Parameters.AddWithValue("@sampleTableId", sampleSets[i].SampleTableId);
        cmd.Parameters.AddWithValue("@user", sampleSets[i].User);
        cmd.Parameters.AddWithValue("@sampleName", sampleSets[i].SampleName);
        cmd.Parameters.AddWithValue("@asPos", sampleSets[i].ASPos);
        cmd.Parameters.AddWithValue("@weight", sampleSets[i].Weight);
        cmd.Parameters.AddWithValue("@sampleTypeId", sampleSets[i].SampleTypeId);
        cmd.Parameters.AddWithValue("@methodId", sampleSets[i].MethodId);
        cmd.Parameters.AddWithValue("@protein", sampleSets[i].Protein);
        cmd.Parameters.AddWithValue("@moisture", sampleSets[i].Moisture);
        cmd.Parameters.AddWithValue("@comment1", sampleSets[i].Comment1);
        cmd.Parameters.AddWithValue("@comment2", sampleSets[i].Comment2);
        cmd.Parameters.AddWithValue("@asPosRank", sampleSets[i].ASPosRank);
        cmd.Parameters.AddWithValue("@sampleId", sampleSets[i].SampleID);

        var list = dao.ExecuteNonQueryParameterised(cmd, connection); // executes the sql query with the parameters provided

    }

    
    
}