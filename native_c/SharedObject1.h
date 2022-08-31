#pragma once

extern "C"
{
    namespace SharedObject1
    {

        float Foopluginmethod();
        struct Color32;
        void ProcessImage(Color32** rawImage1, int width1, int height1);

        bool creatMat(Color32** rawImage1, int width1, int height1);

        bool saveMat(Color32** rawImage1, int width1, int height1, char* str);

        double changeMat(unsigned char* data, int width, int height);

        double check_value(unsigned char* input_data, unsigned char* output_data, int width, int height);

        bool foo(char* str);

        bool copyMessage(char* _input, char* _output);

        bool change_array(double c_point[], double x[], double y[]);

        bool matching_input(unsigned char* input_data, int width, int height, double c_point[], double mx[], double my[], double set_angle);

        int input_vec_mat(unsigned char* input_data, int width, int height);

        double BackGround(unsigned char* input_data, int width, int height);


        //int recive(jbyteArray * jb);
    }
}
