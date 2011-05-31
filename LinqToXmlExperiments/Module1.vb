Module Module1

    Private xNewImportXml As XElement
    Private xLastImportXml As XElement

    Private aPt As LinqToXmlExperiments.Patient
    Private aPts As System.Linq.IQueryable(Of LinqToXmlExperiments.Patient)

    Private gPatientId As String = Nothing

    Sub Main()
        Console.WriteLine("Importing file...")
        importFile()
        Console.WriteLine("File Imported. Press any key to exit")
        Console.ReadKey()
        Exit Sub


        Dim yep As Integer = 0
        Dim nup As Integer = 0
        Dim aYep As Integer = 0
        Dim aNup As Integer = 0
        Dim patId As Integer = 0
        Dim str As String = Nothing
        Dim gPatientId As String = Nothing
        Dim G As String = Nothing
        Dim O As String = Nothing
        Dim A As String = Nothing

        'Dim xNewImportXml As XElement
        'Dim xLastImportXml As XElement

        'Load xml files
        xNewImportXml = XElement.Load(System.IO.File.OpenRead("c:\patients.xml"), LoadOptions.None)
        xLastImportXml = XElement.Load(System.IO.File.OpenRead("c:\patients_copy.xml"), LoadOptions.None)
        Dim qldImport As New QldImportDataContext()

        ' Create Linq to xml queries
        Dim xNewImportPatients As IEnumerable(Of XElement) = From el In xNewImportXml.<patients> Select el
        Dim xLastImportPatients As IEnumerable(Of XElement) = From pat In xLastImportXml.<patients> Select pat



        Dim oPt As System.Xml.Linq.XElement
        'Dim aPt As LinqToXmlExperiments.Patient

        Dim dictOverwriteA As System.Collections.Generic.Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        Dim dictHuman As System.Collections.Generic.Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        Dim dictKeepA As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        Dim dictOfPtDbChanges As Dictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)
        Dim patI As String = "patient_id"

        Try
            ' Iterate through all the patients in the new Import file and compare with existing PatientDB and Last xml import values.
            For Each ptNew As XElement In xNewImportPatients
                ' Variables to store values for comparison
                'Console.WriteLine("ptNew.@patient_id={0}", ptNew.Element(patI).Value)
                gPatientId = ptNew.<patient_id>.Value
                G = ptNew.<surname>.Value

                ' Queries to run for last import and PatientDB matching
                oPt = (From patById In xLastImportXml.<patients> Where patById.<patient_id>.Value = gPatientId Select patById).FirstOrDefault()
                aPt = (From patientAllergyQld In qldImport.PatientsAllergyQLDs Where patientAllergyQld.patient_id_qld.ToString() = gPatientId Select patientAllergyQld.Patient).FirstOrDefault()
                Dim aPts As System.Linq.IQueryable(Of LinqToXmlExperiments.Patient) = From pAQ In qldImport.PatientsAllergyQLDs Where pAQ.patient_id_qld.ToString = gPatientId Select pAQ.Patient

                If aPts.Count() > 1 Then
                    Console.WriteLine("Query returned {0} results!", aPts.Count())
                    Dim ptIds As String = Nothing
                    For Each pt As Patient In aPts
                        ptIds = ptIds + pt.PatientID + " "
                    Next
                    Console.WriteLine("Patients with multiple ids {0}", ptIds)
                Else

                End If

                O = oPt.<surname>.Value
                A = aPt.LastName

                If gPatientId.Equals("8864") OrElse gPatientId.Equals("13638") OrElse gPatientId.Equals("11080") Then
                    Console.WriteLine("gPtId:{0} O:{1} A:{2} G:{3}", gPatientId, O, A, G)
                End If


                '' PatientDB data matching
                'If Not StringsMatch(importLastName, allergyDbById.LastName) Then
                '    ' New import and AllergyDB differ. Add field to list and / or set Boolean
                '    If Not dictOverwriteA.ContainsKey(importPatientId) Then
                '        dictOverwriteA.Add(importPatientId, New List(Of String))
                '    End If
                '    dictOverwriteA.Item(importPatientId).Add("Surname")

                '    dictOfPtDbChanges.Add("Surname", True)


                '    aNup += 1
                '    'Console.WriteLine("Mismatch: {2} {0} {1} : {3} {4} {5} ", allergyDbById.FirstName, allergyDbById.LastName, allergyDbById.PatientsAllergyQLDs.First.patient_id_qld, importPatientId, ptNew.<first_name>.Value, ptNew.<surname>.Value)
                'Else
                '    aYep += 1
                'End If


                ' G == A ?
                If StringsMatch(G, A) Then
                    ' G = A. So no changes.
                Else
                    ' G == O ?
                    If StringsMatch(G, O) Then
                        ' A == O ?
                        If StringsMatch(A, O) Then
                            ' Error! G = A, G = O, A = O Can't happen!
                        Else
                            ' 2 Only Allergy value changed. Keep allergy value
                            AddToValueList(dictKeepA, gPatientId, "Surname")
                        End If
                    Else
                        ' A == O ?
                        If StringsMatch(A, O) Then
                            ' 1. Only Genie value changed. Overwrite Allergy with Genie value
                            AddToValueList(dictOverwriteA, gPatientId, "Surname")
                        Else
                            ' 3. Both Genie and Allergy changed. Need Human
                            AddToValueList(dictHuman, gPatientId, "Surname")
                        End If
                    End If
                End If

                '' Last import matching
                'If Not StringsMatch(lastPtById.<surname>.Value, importLastName) Then
                '    ' New import and last import differ. Check if this field also changed in PatientDB
                '    If dictOverwriteA.ContainsKey(importPatientId) Then
                '        If dictOverwriteA.Item(importPatientId).Contains("Surname") Then
                '            ' Field is changed in Patientdb too. Add to list for human perusal.
                '            If Not dictHuman.ContainsKey(importPatientId) Then
                '                dictHuman.Add(importPatientId, New List(Of String))
                '            End If
                '            dictHuman.Item(importPatientId).Add("Surname")
                '        Else
                '            'Field has NOT changed in Patientdb. Add to single change list
                '            If Not dictKeepA.ContainsKey(importPatientId) Then
                '                dictKeepA.Add(importPatientId, New List(Of String))
                '            End If
                '            dictKeepA.Item(importPatientId).Add("Surname")
                '        End If
                '    Else
                '        ' Again, no diff between import and PatientDB. Add to single change list
                '        If Not dictKeepA.ContainsKey(importPatientId) Then
                '            dictKeepA.Add(importPatientId, New List(Of String))
                '        End If
                '        dictKeepA.Item(importPatientId).Add("Surname")
                '    End If

                '    nup += 1
                'Else
                '    ' No change between new and last imports. Move along.
                '    yep += 1
                'End If




                'For Each ptLast As XElement In patientById
                '    ' Compare the whole record (of a patient) (opt)
                '    If ptLast.Value <> ptNew.Value Then
                '        'One of the child elements is different
                '        nup += 1

                '        ' Start comparing fields. Compare every field necessary

                '        If ptNew.<full_name>.Value <> ptLast.<full_name>.Value Then
                '            ' Add these elements to another xml document (in memory?) or data structure.
                '            Console.WriteLine("Name mis-match. Import: patient_id={0}; name={1} Last: patient_id={2}; name={3}", ptNew.<patient_id>.Value, ptNew.<full_name>.Value, ptLast.<patient_id>.Value, ptLast.<full_name>.Value)
                '        End If
                '    Else
                '        'Whole record matches
                '        yep += 1
                '    End If
                'Next



                ' Hey dumbAss! You have to match on the qld_patient_id, not the allergy patientID!!!!
                'Dim dumbAssQuery = From patient In qldImport.Patients Where (patient.PatientID.ToString() = strPatientID) Select patient



                'For Each pt In fatAss
                '    If Not strLastName.Equals(pt.LastName) Then 'Note that this is case sensitive matching!!!!
                '        Console.WriteLine("Mismatch: {2} {0} {1} : {3} {4} {5} ", pt.FirstName, pt.LastName, pt.PatientsAllergyQLDs.First.patient_id_qld, strPatientID, ptNew.<first_name>.Value, ptNew.<surname>.Value)
                '    End If
                'Next


                ' 3. Store or writeout the different values.

                'strPatientID = pt.<patient_id>.Value
                'str = (pt.<dob>.Value).Replace(" ", "").Substring(0, 10)
                'Console.WriteLine(strPatientID + " " + str)
            Next
        Catch ex As Exception

        End Try


        For Each keyb As String In dictHuman.Keys
            For Each valb As String In dictHuman.Item(keyb)
                Console.WriteLine("G and A changed. Human check needed for {0}:{1} ", keyb, valb)
            Next
        Next

        For Each keyb As String In dictKeepA.Keys
            For Each valb As String In dictKeepA.Item(keyb)
                Console.WriteLine("A changed, not G, Keep A {0}:{1}", keyb, valb)
            Next
        Next

        For Each keyb As String In dictOverwriteA.Keys
            For Each valb As String In dictOverwriteA.Item(keyb)
                Console.WriteLine("G changed, not A, Overwrite A on {0}:{1}", keyb, valb)
            Next
        Next

        'For Each lst As List(Of String) In dictDiffLastImport.Values

        'Next
        Console.WriteLine("Last Matches = {0}. Last Non-Matches = {1}", yep, nup)
        Console.WriteLine("Allergy Matches = {0}. Allergy Non-Matches = {1}", aYep, aNup)
        Console.WriteLine("Press any key to quit")
        Console.ReadKey()
    End Sub

    Private Function StringsMatch(ByVal s1 As String, ByVal s2 As String) As Boolean
        If s1 IsNot Nothing AndAlso s2 IsNot Nothing Then
            If s1.Replace(" ", "").ToLower() = s2.Replace(" ", "").ToLower() Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Private Sub AddToValueList(ByVal dict As Dictionary(Of String, List(Of String)), ByVal key As String, ByVal val As String)
        If Not dict.Keys.Contains(key) Then
            dict.Add(key, New List(Of String))
        End If
        dict.Item(key).Add(val)
    End Sub

    Private aQldChildren As IEnumerable(Of LinqToXmlExperiments.PatientsAllergyQLD)

    Private Sub importFile()
        xNewImportXml = XElement.Load(System.IO.File.OpenRead("c:\patients.xml"), LoadOptions.None)
        Dim qldImport As New QldImportDataContext()
        Dim xNewImportPatients As IEnumerable(Of XElement) = From el In xNewImportXml.<patients> Select el

        ' These are the patientIds that have more than one Qld Patient associated with them
        ' SELECT PatientID FROM dbo.PatientsAllergyQLD GROUP BY PatientID HAVING COUNT(PatientID) > 1
        Dim twoQldOneAllergy = From p In qldImport.PatientsAllergyQLDs Group p By p.PatientID Into Count() Where Count > 1 Select PatientID


        ' Iterate through each patient in the import file
        ' and compare with the previous import file and the database records
        For Each pt As XElement In xNewImportPatients
            gPatientId = pt.Element("patient_id").Value
            Console.WriteLine(gPatientId)
            ' First we need to determine if this patient has a corresponding record in AllergyDB
            ' O records = create new
            ' 1 record = compare and update or resolve
            ' More than one record = error! You cannot map a single Qld Patient to more than one Allergy Pt. The duplicate records must be resolved.
            Try
                ' TODO - what is the default value for a patient? Null?
                aPt = (From pq In qldImport.PatientsAllergyQLDs Where pq.patient_id_qld.ToString() = gPatientId Select pq.Patient).SingleOrDefault()
            Catch nullExcept As ArgumentNullException
                ' The first part of the query, or some part of it (before the SingleOrDefault call) is null. Could this mean no match? 
                Console.WriteLine("Null Exception for ID:{0} {1}", gPatientId, nullExcept.Message)
                Exit For
            Catch invalidEx As InvalidOperationException
                ' Contains more than one element. This shouldn't happen, because a Qld record should have only one Allergy record, although the Allergy record could have more than one Qld record.
                ' Thus Qld record is on "many" side of relationship, and Allergy record is "parent".
                ' Skip this record and log the error
                Console.WriteLine(invalidEx.Message)
                Exit For
            End Try

            If aPt Is Nothing Then ' We are assuming the default value for a Patient is Nothing
                ' need to insert a new patient

            Else
                ' need to update existing patient


            End If

            'gPatientId = pt.Element("patient_id").Value
            'aPts = From pAQ In qldImport.PatientsAllergyQLDs Where pAQ.patient_id_qld.ToString = gPatientId Select pAQ.Patient
            'Dim ct As Integer = aPts.Count
            'If ct > 1 Then
            '    ' a qld_id should never be linked to more than one Patientdb.Patients.PatientId
            'ElseIf ct = 1 Then
            '    ' This is what we want. 
            'ElseIf ct = 0 Then
            '    Console.WriteLine("New Patient! with ID: {0}", gPatientId)
            'End If

            ' We have a number of conversions.
            ' 1. xml string to integer (patient_id)
            ' 2. xml string to Bigint (medicare_no)
            ' 3. xml string to Date (dob, referral_date, creation_date
            ' 4. xml number to duration_of_referral string
            ' 5. SPLIT the doctor's name
            ' 6. CONCATENATE the address fields
            ' 

            ' Other functions include
            ' 10. Find the DoctorId, SuburbId
            ' 11. Find the corresponding PatientId to patient_id (Qld)
            ' 12.

            ' 1. Convert string to integer
            Dim patient_id As Integer = Integer.Parse(pt.Element("patient_id").Value)

        Next



    End Sub
End Module
