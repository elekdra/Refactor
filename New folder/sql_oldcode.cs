public List<long> UpdateSampleSets(SampleSetModel[] sampleSets)
{   DataAccess dao = new DataAccess();
    List<long> duplicateSampleSets = new List<long>();
    for (int i = sampleSets.Length - 1; i >= 0; i--)
    {

        string checkDuplicateQuery = "select COUNT(*) from tb_sample_sets where asPos = '" + sampleSets[i].ASPos + "' and sampleID <> '" + sampleSets[i].SampleID + "'";
        string checkForCal = "Select createdFromcal from tb_sample_sets where sampleTableID='" + sampleSets[i].SampleTableId + "'";

        var list1 = dao.ExecuteQuery(checkForCal);

        bool isCalTable = (list1[0]["createdFromcal"] == "True") ? true : false;
        //Console.WriteLine(isCalTable);




        string query;

        if (sampleSets[i].SampleTypeId == 1 && isCalTable)
        {
            query = "update tb_sample_sets set sampleTableId = '" + sampleSets[i].SampleTableId + "', user ='" + sampleSets[i].User + "', sampleName ='" + sampleSets[i].SampleName +
      "', asPos ='" + sampleSets[i].ASPos + "', weight ='" + sampleSets[i].Weight + "', sampleTypeId ='" + sampleSets[i].SampleTypeId + "', methodId ='" + sampleSets[i].MethodId +
       "', calibrationId ='" + sampleSets[i].CalibrationId + "', protein ='" + sampleSets[i].Protein + "', moisture ='" + sampleSets[i].Moisture +
      "', comment1 ='" + sampleSets[i].Comment1.ToString().Replace("'", "''") + "', comment2 ='" + sampleSets[i].Comment2.ToString().Replace("'", "''") + "', asPosRank = '" + sampleSets[i].ASPosRank +
      "' where sampleID = '" + sampleSets[i].SampleID + "'";

        }
        else if (sampleSets[i].SampleTypeId == 1)
        {
            query = "update tb_sample_sets set sampleTableId = '" + sampleSets[i].SampleTableId + "', user ='" + sampleSets[i].User + "', sampleName ='" + sampleSets[i].SampleName +
       "', asPos ='" + sampleSets[i].ASPos + "', weight ='" + sampleSets[i].Weight + "', sampleTypeId ='" + sampleSets[i].SampleTypeId + "', methodId ='" + sampleSets[i].MethodId +
       "', standardId =" + "null" + ", calibrationId =" + "null" + ", protein ='" + sampleSets[i].Protein + "', moisture ='" + sampleSets[i].Moisture +
       "', comment1 ='" + sampleSets[i].Comment1.ToString().Replace("'", "''") + "', comment2 ='" + sampleSets[i].Comment2.ToString().Replace("'", "''") + "', asPosRank = '" + sampleSets[i].ASPosRank +
       "' where sampleID = '" + sampleSets[i].SampleID + "'";
            sampleSets[i].StandardId = null;
            sampleSets[i].CalibrationId = null;
        }
        else if (sampleSets[i].SampleTypeId == 2)
        {
            query = "update tb_sample_sets set sampleTableId = '" + sampleSets[i].SampleTableId + "', user ='" + sampleSets[i].User + "', sampleName ='" + sampleSets[i].SampleName +
         "', asPos ='" + sampleSets[i].ASPos + "', weight ='" + sampleSets[i].Weight + "', sampleTypeId ='" + sampleSets[i].SampleTypeId + "', methodId ='" + sampleSets[i].MethodId +
         "', standardId =" + "null" + ", calibrationId ='" + sampleSets[i].CalibrationId + "', protein ='" + sampleSets[i].Protein + "', moisture ='" + sampleSets[i].Moisture +
         "', comment1 ='" + sampleSets[i].Comment1.ToString().Replace("'", "''") + "', comment2 ='" + sampleSets[i].Comment2.ToString().Replace("'", "''") + "', asPosRank = '" + sampleSets[i].ASPosRank +
         "' where sampleID = '" + sampleSets[i].SampleID + "'";
            sampleSets[i].StandardId = null;

        }
        else
        {
            query = "update tb_sample_sets set sampleTableId = '" + sampleSets[i].SampleTableId + "', user ='" + sampleSets[i].User + "', sampleName ='" + sampleSets[i].SampleName +
       "', asPos ='" + sampleSets[i].ASPos + "', weight ='" + sampleSets[i].Weight + "', sampleTypeId ='" + sampleSets[i].SampleTypeId + "', methodId ='" + sampleSets[i].MethodId +
       "', standardId ='" + sampleSets[i].StandardId + "', calibrationId ='" + sampleSets[i].CalibrationId + "', protein ='" + sampleSets[i].Protein + "', moisture ='" + sampleSets[i].Moisture +
       "', comment1 ='" + sampleSets[i].Comment1.ToString().Replace("'", "''") + "', comment2 ='" + sampleSets[i].Comment2.ToString().Replace("'", "''") + "', asPosRank = '" + sampleSets[i].ASPosRank +
       "' where sampleID = '" + sampleSets[i].SampleID + "'";
        }

        // Console.WriteLine(query);
        var list = dao.ExecuteNonQuery(query);

    }
   
    return duplicateSampleSets;
}
